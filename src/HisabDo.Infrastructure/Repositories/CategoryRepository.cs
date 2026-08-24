using HisabDo.Application.Repositories;
using HisabDo.Domain.Entities;
using HisabDo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HisabDo.Infrastructure.Repositories;

public class CategoryRepository(HisabDoDbContext context) : ICategoryRepository
{
    public async Task<(List<Category> Items, int TotalCount)> GetAllAsync(int userId, int page = 1, int pageSize = 50)
    {
        var query = context.Categories
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

    public async Task<Category?> GetByIdAsync(int userId, int id)
    {
        return await context.Categories
            .FirstOrDefaultAsync(c => c.UserId == userId && c.Id == id && !c.IsDeleted);
    }

    public async Task<bool> NameExistsAsync(int userId, string name, int? excludeId = null)
    {
        return await context.Categories
            .AnyAsync(c => c.UserId == userId
                && !c.IsDeleted
                && c.Name.ToLower() == name.ToLower()
                && (!excludeId.HasValue || c.Id != excludeId.Value));
    }

    public async Task<bool> HasTransactionsAsync(int id)
    {
        return await context.Transactions
            .AnyAsync(t => t.CategoryId == id && !t.IsDeleted);
    }

    public async Task<Category> AddAsync(Category category)
    {
        context.Categories.Add(category);
        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new InvalidOperationException("A concurrency error occurred. Please try again.");
        }
        return category;
    }

    public async Task AddRangeAsync(IEnumerable<Category> categories)
    {
        context.Categories.AddRange(categories);
        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new InvalidOperationException("A concurrency error occurred. Please try again.");
        }
    }

    public async Task<Category> UpdateAsync(Category category)
    {
        context.Categories.Update(category);
        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new InvalidOperationException("A concurrency error occurred. The record was modified by another user. Please refresh and try again.");
        }
        return category;
    }

    public async Task RemoveAsync(Category category)
    {
        category.IsDeleted = true;
        context.Categories.Update(category);
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
