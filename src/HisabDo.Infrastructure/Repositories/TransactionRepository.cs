using HisabDo.Application.Repositories;
using HisabDo.Domain.Entities;
using HisabDo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HisabDo.Infrastructure.Repositories;

public class TransactionRepository(HisabDoDbContext context) : ITransactionRepository
{
    public async Task<List<Transaction>> GetAllAsync(int userId)
    {
        return await context.Transactions
            .Where(t => t.UserId == userId && !t.IsDeleted)
            .Include(t => t.Customer)
            .Include(t => t.Category)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync();
    }

    public async Task<Transaction?> GetByIdAsync(int id)
    {
        return await context.Transactions
            .Include(t => t.Customer)
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
    }

    public async Task<bool> CustomerExistsAsync(int customerId)
    {
        return await context.Customers.AnyAsync(c => c.Id == customerId && !c.IsDeleted);
    }

    public async Task<bool> CategoryExistsAsync(int categoryId)
    {
        return await context.Categories.AnyAsync(c => c.Id == categoryId);
    }

    public async Task<Transaction> AddAsync(Transaction transaction)
    {
        context.Transactions.Add(transaction);
        await context.SaveChangesAsync();
        return transaction;
    }

    public async Task<Transaction> UpdateAsync(Transaction transaction)
    {
        context.Transactions.Update(transaction);
        await context.SaveChangesAsync();
        return transaction;
    }

    public async Task RemoveAsync(Transaction transaction)
    {
        transaction.IsDeleted = true;
        context.Transactions.Update(transaction);
        await context.SaveChangesAsync();
    }
}