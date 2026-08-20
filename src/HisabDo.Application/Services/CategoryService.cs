using HisabDo.Application.DTOs;
using HisabDo.Application.Repositories;
using HisabDo.Domain.Entities;

namespace HisabDo.Application.Services;

public class CategoryService(ICategoryRepository repository) : ICategoryService
{
    public async Task<IEnumerable<CategoryDto>> GetAllAsync(int userId)
    {
        var categories = await repository.GetAllAsync(userId);
        return categories.Select(ToDto);
    }

    public async Task<CategoryDto?> GetByIdAsync(int id)
    {
        var category = await repository.GetByIdAsync(id);
        return category == null ? null : ToDto(category);
    }

    public async Task<CategoryDto> CreateAsync(int userId, CreateCategoryDto dto)
    {
        await EnsureNameIsUniqueAsync(userId, dto.Name);

        var category = new Category
        {
            UserId = userId,
            Name = dto.Name,
            IsDefault = false
        };

        await repository.AddAsync(category);
        return ToDto(category);
    }

    public async Task<CategoryDto> UpdateAsync(int id, CreateCategoryDto dto)
    {
        var category = await repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"No category found with ID: {id}");

        if (category.IsDefault)
        {
            throw new InvalidOperationException("Default category cannot be updated.");
        }

        await EnsureNameIsUniqueAsync(category.UserId, dto.Name, id);

        category.Name = dto.Name;
        await repository.UpdateAsync(category);
        return ToDto(category);
    }

    public async Task DeleteAsync(int id)
    {
        var category = await repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"No category found with ID: {id}");

        if (category.IsDefault)
        {
            throw new InvalidOperationException("Default category cannot be deleted.");
        }

        if (await repository.HasTransactionsAsync(id))
        {
            throw new InvalidOperationException("Category is used by transactions and cannot be deleted.");
        }

        await repository.RemoveAsync(category);
    }

    private async Task EnsureNameIsUniqueAsync(int userId, string name, int? excludeId = null)
    {
        if (await repository.NameExistsAsync(userId, name, excludeId))
        {
            throw new InvalidOperationException($"A category with the name '{name}' already exists.");
        }
    }

    private static CategoryDto ToDto(Category category)
    {
        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            IsDefault = category.IsDefault,
            CreatedAt = category.CreatedAt
        };
    }
}
