using HisabDo.Application.DTOs;

namespace HisabDo.Application.Services;

public interface ICategoryService
{
    Task<PaginatedResult<CategoryDto>> GetAllAsync(int userId, int page = 1, int pageSize = 50);
    Task<CategoryDto?> GetByIdAsync(int userId, int id);
    Task<CategoryDto> CreateAsync(int userId, CreateCategoryDto dto);
    Task<CategoryDto> UpdateAsync(int userId, int id, CreateCategoryDto dto);
    Task DeleteAsync(int userId, int id);
}
