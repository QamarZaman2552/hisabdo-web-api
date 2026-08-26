using HisabDo.Application.DTOs;
using HisabDo.Application.Repositories;
using HisabDo.Domain.Entities;
using HisabDo.Domain.Enums;
using HisabDo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HisabDo.Infrastructure.Repositories;

public class BackupRepository(HisabDoDbContext context) : IBackupRepository
{
    public async Task<BackupFileDto> ExportAsync(int userId)
    {
        var categories = await context.Categories
            .Where(c => c.UserId == userId && !c.IsDeleted)
            .OrderBy(c => c.Id)
            .ToListAsync();

        var customers = await context.Customers
            .Where(c => c.UserId == userId && !c.IsDeleted)
            .OrderBy(c => c.Id)
            .ToListAsync();

        var transactions = await context.Transactions
            .Where(t => t.UserId == userId && !t.IsDeleted)
            .OrderBy(t => t.Id)
            .ToListAsync();

        var setting = await context.Settings
            .FirstOrDefaultAsync(s => s.UserId == userId && !s.IsDeleted);

        return new BackupFileDto
        {
            ExportedAt = DateTime.UtcNow,
            Settings = setting == null ? null : new BackupSettingsDto
            {
                CurrencyCode = setting.CurrencyCode,
                LanguageCode = setting.LanguageCode
            },
            Categories = categories.Select(c => new BackupCategoryDto
            {
                OriginalId = c.Id,
                Name = c.Name,
                IsDefault = c.IsDefault
            }).ToList(),
            Customers = customers.Select(c => new BackupCustomerDto
            {
                OriginalId = c.Id,
                Name = c.Name,
                Phone = c.Phone ?? string.Empty,
                Email = c.Email ?? string.Empty,
                Notes = c.Notes ?? string.Empty
            }).ToList(),
            Transactions = transactions.Select(t => new BackupTransactionDto
            {
                Amount = t.Amount,
                Type = (int)t.Type,
                Note = t.Note,
                TransactionDate = t.TransactionDate,
                OriginalCategoryId = t.CategoryId,
                OriginalCustomerId = t.CustomerId
            }).ToList()
        };
    }

    public async Task<RestoreResultDto> RestoreAsync(int userId, BackupFileDto data, bool replaceExisting)
    {
        var result = new RestoreResultDto();
        List<string> pendingAttachmentUrls = [];

        await using var dbTransaction = await context.Database.BeginTransactionAsync();
        try
        {
            if (replaceExisting)
            {
                pendingAttachmentUrls = await ClearAllDatabaseAsync(userId);
            }
        var categoryMap = new Dictionary<int, int>();
        foreach (var dto in data.Categories ?? [])
        {
            var exists = await context.Categories.AnyAsync(c =>
                c.UserId == userId && !c.IsDeleted && c.Name.ToLower() == dto.Name.ToLower());

            if (exists)
            {
                var existingId = await context.Categories
                    .Where(c => c.UserId == userId && !c.IsDeleted && c.Name.ToLower() == dto.Name.ToLower())
                    .Select(c => c.Id)
                    .FirstAsync();
                categoryMap[dto.OriginalId] = existingId;
                result.CategoriesSkipped++;
                continue;
            }

            var category = new Category
            {
                UserId = userId,
                Name = dto.Name,
                IsDefault = dto.IsDefault
            };
            context.Categories.Add(category);
            await context.SaveChangesAsync();
            if (dto.OriginalId > 0)
            {
                categoryMap[dto.OriginalId] = category.Id;
            }
            result.CategoriesImported++;
        }

        var customerMap = new Dictionary<int, int>();
        foreach (var dto in data.Customers ?? [])
        {
            var customer = new Customer
            {
                UserId = userId,
                Name = dto.Name,
                Phone = dto.Phone,
                Email = dto.Email,
                Notes = dto.Notes
            };
            context.Customers.Add(customer);
            await context.SaveChangesAsync();
            if (dto.OriginalId != 0)
            {
                customerMap[dto.OriginalId] = customer.Id;
            }
            result.CustomersImported++;
        }

        foreach (var dto in data.Transactions ?? [])
        {
            if (!categoryMap.TryGetValue(dto.OriginalCategoryId, out var categoryId))
            {
                result.TransactionsSkipped++;
                continue;
            }

            int? customerId = null;
            if (dto.OriginalCustomerId.HasValue &&
                customerMap.TryGetValue(dto.OriginalCustomerId.Value, out var mappedCustomerId))
            {
                customerId = mappedCustomerId;
            }

            context.Transactions.Add(new Transaction
            {
                UserId = userId,
                CustomerId = customerId,
                CategoryId = categoryId,
                Type = (TransactionType)dto.Type,
                Amount = dto.Amount,
                Note = dto.Note,
                TransactionDate = dto.TransactionDate == default ? DateTime.UtcNow : dto.TransactionDate
            });
            result.TransactionsImported++;
        }

        if (result.TransactionsImported > 0)
        {
            await context.SaveChangesAsync();
        }

        if (data.Settings != null)
        {
            var setting = await context.Settings
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => !s.IsDeleted)
                .ThenBy(s => s.Id)
                .FirstOrDefaultAsync();

            if (setting == null)
            {
                context.Settings.Add(new Setting
                {
                    UserId = userId,
                    CurrencyCode = data.Settings.CurrencyCode,
                    LanguageCode = data.Settings.LanguageCode
                });
            }
            else
            {
                setting.CurrencyCode = data.Settings.CurrencyCode;
                setting.LanguageCode = data.Settings.LanguageCode;
                setting.IsDeleted = false;
            }
            await context.SaveChangesAsync();
        }

        await dbTransaction.CommitAsync();
        DeleteAttachmentFiles(pendingAttachmentUrls);
        return result;
        }
        catch
        {
            await dbTransaction.RollbackAsync();
            throw;
        }
    }

    public async Task ClearAllAsync(int userId)
    {
        var attachmentUrls = await ClearAllDatabaseAsync(userId);
        DeleteAttachmentFiles(attachmentUrls);
    }

    private async Task<List<string>> ClearAllDatabaseAsync(int userId)
    {
        var attachmentUrls = await context.Transactions
            .Where(t => t.UserId == userId && !t.IsDeleted && t.AttachmentUrl != null)
            .Select(t => t.AttachmentUrl!)
            .ToListAsync();

        await context.Transactions
            .Where(t => t.UserId == userId && !t.IsDeleted)
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.IsDeleted, true));

        await context.Customers
            .Where(c => c.UserId == userId && !c.IsDeleted)
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.IsDeleted, true));

        await context.Categories
            .Where(c => c.UserId == userId && !c.IsDeleted)
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.IsDeleted, true));

        return attachmentUrls;
    }

    private static void DeleteAttachmentFiles(IEnumerable<string> urls)
    {
        foreach (var url in urls)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", url.TrimStart('/'));
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}
