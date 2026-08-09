using HisabDo.Application.DTOs;

namespace HisabDo.Application.Services;

public interface ICustomerService
{
    Task<IEnumerable<CustomerDto>> GetAllAsync(int userId);
    Task<CustomerDto?> GetByIdAsync(int id);
    Task<CustomerDto> CreateAsync(int userId, CreateCustomerDto dto);
    Task<CustomerDto> UpdateAsync(int id, CreateCustomerDto dto);
    Task DeleteAsync(int id);
}