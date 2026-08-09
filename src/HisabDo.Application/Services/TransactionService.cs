using HisabDo.Application.DTOs;
using HisabDo.Application.Repositories;
using HisabDo.Domain.Entities;

namespace HisabDo.Application.Services;

public class TransactionService(ITransactionRepository repository) : ITransactionService
{
    public async Task<IEnumerable<TransactionDto>> GetAllAsync(int userId)
    {
        var transactions = await repository.GetAllAsync(userId);
        return transactions.Select(ToDto);
    }

    public async Task<TransactionDto?> GetByIdAsync(int id)
    {
        var transaction = await repository.GetByIdAsync(id);
        return transaction == null ? null : ToDto(transaction);
    }

    public async Task<TransactionDto> CreateAsync(int userId, CreateTransactionDto dto)
    {
        await EnsureCustomerAndCategoryExistAsync(dto.CustomerId, dto.CategoryId);

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

    public async Task<TransactionDto> UpdateAsync(int id, CreateTransactionDto dto)
    {
        var transaction = await repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"No transaction found with ID: {id}");

        await EnsureCustomerAndCategoryExistAsync(dto.CustomerId, dto.CategoryId);

        transaction.CustomerId = dto.CustomerId;
        transaction.CategoryId = dto.CategoryId;
        transaction.Type = dto.Type;
        transaction.Amount = dto.Amount;
        transaction.Note = dto.Note;
        transaction.TransactionDate = dto.TransactionDate ?? transaction.TransactionDate;

        await repository.UpdateAsync(transaction);
        return await GetByIdAsync(id) ?? ToDto(transaction);
    }

    public async Task DeleteAsync(int id)
    {
        var transaction = await repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"No transaction found with ID: {id}");

        await repository.RemoveAsync(transaction);
    }

    private async Task EnsureCustomerAndCategoryExistAsync(int customerId, int categoryId)
    {
        if (!await repository.CustomerExistsAsync(customerId))
        {
            throw new InvalidOperationException($"No customer found with ID: {customerId}.");
        }

        if (!await repository.CategoryExistsAsync(categoryId))
        {
            throw new InvalidOperationException($"No category found with ID: {categoryId}.");
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
            TransactionDate = transaction.TransactionDate
        };
    }
}