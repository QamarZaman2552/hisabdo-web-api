using HisabDo.Domain.Entities;

namespace HisabDo.Application.Repositories;

public interface ISettingRepository
{
    Task<Setting?> GetByUserIdAsync(int userId);
    Task<Setting> AddOrUpdateAsync(Setting setting);
    Task RemoveAsync(Setting setting);
}
