using HisabDo.Domain.Entities;

namespace HisabDo.Application.Repositories;

public interface ITransactionRepository
{
    Task<List<Transaction>> GetAllAsync(int userId);
    Task<Transaction?> GetByIdAsync(int id);
    Task<bool> CustomerExistsAsync(int customerId);
    Task<bool> CategoryExistsAsync(int categoryId);
    Task<Transaction> AddAsync(Transaction transaction);
    Task<Transaction> UpdateAsync(Transaction transaction);
    Task RemoveAsync(Transaction transaction);
}