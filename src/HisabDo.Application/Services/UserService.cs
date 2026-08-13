using HisabDo.Application.DTOs;
using HisabDo.Application.Repositories;

namespace HisabDo.Application.Services;

public class UserService(IUserRepository repository) : IUserService
{
    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var users = await repository.GetUsersAsync();

        return users.Select(u => new UserDto
        {
            Id = u.Id,
            FullName = u.FullName,
            Email = u.Email,
            Role = u.Role,
            CreatedAt = u.CreatedAt
        });
    }
}