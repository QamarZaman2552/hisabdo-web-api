using HisabDo.Application.Repositories;
using HisabDo.Domain.Entities;
using HisabDo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HisabDo.Infrastructure.Repositories;

public class CustomerRepository(HisabDoDbContext context) : ICustomerRepository
{
    public async Task<(List<Customer> Items, int TotalCount)> GetAllAsync(int userId, int page = 1, int pageSize = 50)
    {
        var query = context.Customers
            .Where(c => c.UserId == userId && !c.IsDeleted);

        var totalCount = await query.CountAsync();

        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var items = await query
            .OrderBy(c => c.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<Customer?> GetByIdAsync(int userId, int id)
    {
        return await context.Customers
            .FirstOrDefaultAsync(c => c.UserId == userId && c.Id == id && !c.IsDeleted);
    }

    public async Task<bool> HasTransactionsAsync(int customerId)
    {
        return await context.Transactions
            .AnyAsync(t => t.CustomerId == customerId && !t.IsDeleted);
    }

    public async Task<Customer> AddAsync(Customer customer)
    {
        context.Customers.Add(customer);
        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new InvalidOperationException("A concurrency error occurred. Please try again.");
        }
        return customer;
    }

    public async Task<Customer> UpdateAsync(Customer customer)
    {
        context.Customers.Update(customer);
        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new InvalidOperationException("A concurrency error occurred. The record was modified by another user. Please refresh and try again.");
        }
        return customer;
    }

    public async Task RemoveAsync(Customer customer)
    {
        customer.IsDeleted = true;
        context.Customers.Update(customer);
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