using HisabDo.Application.DTOs;
using HisabDo.Application.Repositories;
using HisabDo.Domain.Entities;

namespace HisabDo.Application.Services;

public class TransactionService(ITransactionRepository repository) : ITransactionService
{
    public async Task<PaginatedResult<TransactionDto>> GetAllAsync(int userId, TransactionFilterDto filter)
    {
        var (transactions, totalCount) = await repository.GetAllAsync(userId, filter);
        return new PaginatedResult<TransactionDto>
        {
            Items = transactions.Select(ToDto).ToList(),
            Page = filter.Page,
            PageSize = filter.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<IEnumerable<TransactionDto>> GetByCategoryAsync(int userId, int categoryId)
    {
        if (!await repository.CategoryExistsAsync(userId, categoryId))
        {
            throw new KeyNotFoundException($"No category found with ID: {categoryId}");
        }

        var transactions = await repository.GetByCategoryAsync(userId, categoryId);
        return transactions.Select(ToDto);
    }

    public async Task<TransactionDto?> GetByIdAsync(int id)
    {
        var transaction = await repository.GetByIdAsync(id);
        return transaction == null ? null : ToDto(transaction);
    }

    public async Task<TransactionDto> CreateAsync(int userId, CreateTransactionDto dto)
    {
        await EnsureCustomerAndCategoryExistAsync(userId, dto.CustomerId, dto.CategoryId);
        EnsureDateIsNotInFuture(dto.TransactionDate);

        var transaction = new Transaction
        {
            UserId = userId,
            CustomerId = dto.CustomerId,
            CategoryId = dto.CategoryId,
            Type = dto.Type,
            Amount = dto.Amount,
            Note = dto.Note,
            TransactionDate = dto.TransactionDate ?? DateTime.UtcNow
        };

        await repository.AddAsync(transaction);
        return await GetByIdAsync(transaction.Id) ?? ToDto(transaction);
    }

    public async Task<TransactionDto> UpdateAsync(int userId, int id, CreateTransactionDto dto)
    {
        var transaction = await repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"No transaction found with ID: {id}");

        if (transaction.UserId != userId)
        {
            throw new UnauthorizedAccessException("You do not have access to this transaction.");
        }

        await EnsureCustomerAndCategoryExistAsync(userId, dto.CustomerId, dto.CategoryId);
        EnsureDateIsNotInFuture(dto.TransactionDate);

        transaction.CustomerId = dto.CustomerId;
        transaction.CategoryId = dto.CategoryId;
        transaction.Type = dto.Type;
        transaction.Amount = dto.Amount;
        transaction.Note = dto.Note;
        transaction.TransactionDate = dto.TransactionDate ?? transaction.TransactionDate;

        await repository.UpdateAsync(transaction);
        return await GetByIdAsync(id) ?? ToDto(transaction);
    }

    public async Task DeleteAsync(int userId, int id)
    {
        var transaction = await repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"No transaction found with ID: {id}");

        if (transaction.UserId != userId)
        {
            throw new UnauthorizedAccessException("You do not have access to this transaction.");
        }

        if (!string.IsNullOrEmpty(transaction.AttachmentUrl))
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", transaction.AttachmentUrl.TrimStart('/'));
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        await repository.RemoveAsync(transaction);
    }

    public async Task UpdateAttachmentUrlAsync(int userId, int transactionId, string attachmentUrl)
    {
        var transaction = await repository.GetByIdAsync(transactionId)
            ?? throw new KeyNotFoundException($"No transaction found with ID: {transactionId}");

        if (transaction.UserId != userId)
        {
            throw new UnauthorizedAccessException("You do not have access to this transaction.");
        }

        transaction.AttachmentUrl = attachmentUrl;
        await repository.UpdateAsync(transaction);
    }

    public async Task<string?> GetAttachmentUrlAsync(int userId, int transactionId)
    {
        var transaction = await repository.GetByIdAsync(transactionId)
            ?? throw new KeyNotFoundException($"No transaction found with ID: {transactionId}");

        if (transaction.UserId != userId)
        {
            throw new UnauthorizedAccessException("You do not have access to this transaction.");
        }

        return transaction.AttachmentUrl;
    }

    private async Task EnsureCustomerAndCategoryExistAsync(int userId, int customerId, int categoryId)
    {
        if (!await repository.CustomerExistsAsync(userId, customerId))
        {
            throw new InvalidOperationException($"No customer found with ID: {customerId}.");
        }

        if (!await repository.CategoryExistsAsync(userId, categoryId))
        {
            throw new InvalidOperationException($"No category found with ID: {categoryId}.");
        }
    }

    private static void EnsureDateIsNotInFuture(DateTime? transactionDate)
    {
        if (transactionDate.HasValue && transactionDate.Value.Date > DateTime.UtcNow.Date)
        {
            throw new InvalidOperationException("Transaction date cannot be in the future.");
        }
    }

    private static TransactionDto ToDto(Transaction transaction)
    {
        return new TransactionDto
        {
            Id = transaction.Id,
            CustomerId = transaction.CustomerId,
            CustomerName = transaction.Customer?.Name ?? string.Empty,
            CategoryId = transaction.CategoryId,
            CategoryName = transaction.Category?.Name ?? string.Empty,
            Type = transaction.Type,
            Amount = transaction.Amount,
            Note = transaction.Note,
            TransactionDate = transaction.TransactionDate,
            CreatedAt = transaction.CreatedAt,
            AttachmentUrl = transaction.AttachmentUrl
        };
    }
}