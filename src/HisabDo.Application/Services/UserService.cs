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

    public async Task<UserDto?> GetByIdAsync(int userId)
    {
        var user = await repository.GetByIdAsync(userId);
        return user == null ? null : new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<UserDto> UpdateAsync(int userId, UpdateUserDto dto)
    {
        var user = await repository.GetByIdAsync(userId)
            ?? throw new KeyNotFoundException($"No user found with ID: {userId}");

        user.FullName = dto.FullName;
        user.Role = dto.Role ?? user.Role;

        await repository.UpdateAsync(user);

        return new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task DeleteAsync(int userId)
    {
        await repository.DeleteAsync(userId);
    }
}