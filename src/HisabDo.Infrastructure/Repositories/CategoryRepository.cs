using HisabDo.Application.Repositories;
using HisabDo.Domain.Entities;
using HisabDo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HisabDo.Infrastructure.Repositories;

public class CategoryRepository(HisabDoDbContext context) : ICategoryRepository
{
    public async Task<List<Category>> GetAllAsync(int userId)
    {
        return await context.Categories
            .Where(c => c.UserId == userId && !c.IsDeleted)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        return await context.Categories
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
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
        await context.SaveChangesAsync();
        return category;
    }

    public async Task<Category> UpdateAsync(Category category)
    {
        context.Categories.Update(category);
        await context.SaveChangesAsync();
        return category;
    }

    public async Task RemoveAsync(Category category)
    {
        category.IsDeleted = true;
        context.Categories.Update(category);
        await context.SaveChangesAsync();
    }
}
