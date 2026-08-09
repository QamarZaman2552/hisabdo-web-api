using HisabDo.Application.Repositories;
using HisabDo.Domain.Entities;
using HisabDo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HisabDo.Infrastructure.Repositories;

public class CustomerRepository(HisabDoDbContext context) : ICustomerRepository
{
    public async Task<List<Customer>> GetAllAsync(int userId)
    {
        return await context.Customers
            .Where(c => c.UserId == userId && !c.IsDeleted)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<Customer?> GetByIdAsync(int id)
    {
        return await context.Customers
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
    }

    public async Task<Customer> AddAsync(Customer customer)
    {
        context.Customers.Add(customer);
        await context.SaveChangesAsync();
        return customer;
    }

    public async Task<Customer> UpdateAsync(Customer customer)
    {
        context.Customers.Update(customer);
        await context.SaveChangesAsync();
        return customer;
    }

    public async Task RemoveAsync(Customer customer)
    {
        customer.IsDeleted = true;
        context.Customers.Update(customer);
        await context.SaveChangesAsync();
    }
}