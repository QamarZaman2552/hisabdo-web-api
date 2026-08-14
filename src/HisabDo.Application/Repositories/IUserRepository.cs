using HisabDo.Domain.Entities;

namespace HisabDo.Application.Repositories;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(int userId);
    Task<bool> EmailExistsAsync(string email);
    Task<List<User>> GetUsersAsync();
    Task<User> AddAsync(User user);
    Task UpdateAsync(User user);
}

public interface ISettingRepository
{
    Task<Setting?> GetByUserIdAsync(int userId);
    Task<Setting> AddOrUpdateAsync(Setting setting);
    Task RemoveAsync(Setting setting);
}