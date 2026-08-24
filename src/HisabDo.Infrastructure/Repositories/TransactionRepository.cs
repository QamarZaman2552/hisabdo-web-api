using HisabDo.Application.DTOs;
using HisabDo.Application.Repositories;
using HisabDo.Domain.Entities;
using HisabDo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HisabDo.Infrastructure.Repositories;

public class TransactionRepository(HisabDoDbContext context) : ITransactionRepository
{
    public async Task<(List<Transaction> Items, int TotalCount)> GetAllAsync(int userId, TransactionFilterDto filter)
    {
        var query = context.Transactions
            .Where(t => t.UserId == userId && !t.IsDeleted);

        if (filter.Type.HasValue)
        {
            query = query.Where(t => t.Type == filter.Type.Value);
        }

        if (filter.CustomerId.HasValue)
        {
            query = query.Where(t => t.CustomerId == filter.CustomerId.Value);
        }

        if (filter.CategoryId.HasValue)
        {
            query = query.Where(t => t.CategoryId == filter.CategoryId.Value);
        }

        if (filter.FromDate.HasValue)
        {
            query = query.Where(t => t.TransactionDate >= filter.FromDate.Value);
        }

        if (filter.ToDate.HasValue)
        {
            query = query.Where(t => t.TransactionDate <= filter.ToDate.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.ToLower();
            query = query.Where(t => t.Note.ToLower().Contains(search)
                || (t.Customer != null && t.Customer.Name.ToLower().Contains(search)));
        }

        var totalCount = await query.CountAsync();

        var page = Math.Max(1, filter.Page);
        var pageSize = Math.Clamp(filter.PageSize, 1, 100);

        var items = await query
            .Include(t => t.Customer)
            .Include(t => t.Category)
            .OrderByDescending(t => t.TransactionDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<List<Transaction>> GetByCategoryAsync(int userId, int categoryId)
    {
        return await context.Transactions
            .Where(t => t.UserId == userId && !t.IsDeleted && t.CategoryId == categoryId)
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

    public async Task<bool> CustomerExistsAsync(int userId, int customerId)
    {
        return await context.Customers.AnyAsync(c => c.Id == customerId && c.UserId == userId && !c.IsDeleted);
    }

    public async Task<bool> CategoryExistsAsync(int userId, int categoryId)
    {
        return await context.Categories.AnyAsync(c => c.Id == categoryId && c.UserId == userId && !c.IsDeleted);
    }

    public async Task<Transaction> AddAsync(Transaction transaction)
    {
        context.Transactions.Add(transaction);
        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new InvalidOperationException("A concurrency error occurred. Please try again.");
        }
        return transaction;
    }

    public async Task<Transaction> UpdateAsync(Transaction transaction)
    {
        context.Transactions.Update(transaction);
        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new InvalidOperationException("A concurrency error occurred. The record was modified by another user. Please refresh and try again.");
        }
        return transaction;
    }

    public async Task RemoveAsync(Transaction transaction)
    {
        transaction.IsDeleted = true;
        context.Transactions.Update(transaction);
        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new InvalidOperationException("A concurrency error occurred. Please try again.");
        }
    }
}