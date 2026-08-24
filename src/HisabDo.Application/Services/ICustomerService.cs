using HisabDo.Application.DTOs;

namespace HisabDo.Application.Services;

public interface ICustomerService
{
    Task<PaginatedResult<CustomerDto>> GetAllAsync(int userId, int page = 1, int pageSize = 50);
    Task<CustomerDto?> GetByIdAsync(int id);
    Task<CustomerDto> CreateAsync(int userId, CreateCustomerDto dto);
    Task<CustomerDto> UpdateAsync(int userId, int id, CreateCustomerDto dto);
    Task DeleteAsync(int userId, int id);
}