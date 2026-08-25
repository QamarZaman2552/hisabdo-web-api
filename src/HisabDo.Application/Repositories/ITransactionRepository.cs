using HisabDo.Application.DTOs;
using HisabDo.Domain.Entities;

namespace HisabDo.Application.Repositories;

public interface ITransactionRepository
{
    Task<(List<Transaction> Items, int TotalCount)> GetAllAsync(int userId, TransactionFilterDto filter);
    Task<List<Transaction>> GetByCategoryAsync(int userId, int categoryId);
    Task<Transaction?> GetByIdAsync(int userId, int id);
    Task<bool> CustomerExistsAsync(int userId, int customerId);
    Task<bool> CategoryExistsAsync(int userId, int categoryId);
    Task<Transaction> AddAsync(Transaction transaction);
    Task<Transaction> UpdateAsync(Transaction transaction);
    Task RemoveAsync(Transaction transaction);
}