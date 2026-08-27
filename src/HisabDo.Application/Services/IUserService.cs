using HisabDo.Application.DTOs;

namespace HisabDo.Application.Services;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllAsync();
    Task<UserDto?> GetByIdAsync(int userId);
    Task<UserDto> UpdateAsync(int userId, UpdateUserDto dto);
    Task DeleteAsync(int userId);
}