using HisabDo.Application.DTOs;

namespace HisabDo.Application.Services;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllAsync();
}