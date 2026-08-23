using HisabDo.Application.DTOs;

namespace HisabDo.Application.Services;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetAllAsync(int userId);
    Task<CategoryDto?> GetByIdAsync(int id);
    Task<CategoryDto> CreateAsync(int userId, CreateCategoryDto dto);
    Task<CategoryDto> UpdateAsync(int userId, int id, CreateCategoryDto dto);
    Task DeleteAsync(int userId, int id);
}
