using HisabDo.Domain.Entities;

namespace HisabDo.Application.Repositories;

public interface ICategoryRepository
{
    Task<(List<Category> Items, int TotalCount)> GetAllAsync(int userId, int page = 1, int pageSize = 50);
    Task<Category?> GetByIdAsync(int userId, int id);
    Task<bool> NameExistsAsync(int userId, string name, int? excludeId = null);
    Task<bool> HasTransactionsAsync(int id);
    Task<Category> AddAsync(Category category);
    Task<Category> UpdateAsync(Category category);
    Task RemoveAsync(Category category);
}
