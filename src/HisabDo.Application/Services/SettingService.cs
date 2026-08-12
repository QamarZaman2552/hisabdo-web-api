using HisabDo.Application.DTOs;
using HisabDo.Application.Repositories;
using HisabDo.Domain.Entities;

namespace HisabDo.Application.Services;

public class SettingService(ISettingRepository repository) : ISettingService
{
    public async Task<SettingDto?> GetAsync(int userId)
    {
        var setting = await repository.GetByUserIdAsync(userId);
        return setting == null ? null : ToDto(setting);
    }

    public async Task<SettingDto> UpdateAsync(int userId, UpdateSettingDto dto)
    {
        var setting = await repository.GetByUserIdAsync(userId);

        if (setting == null)
        {
            setting = new Setting { UserId = userId };
        }

        setting.CurrencyCode = dto.CurrencyCode;
        setting.LanguageCode = dto.LanguageCode;
        setting.IsDeleted = false;

        await repository.AddOrUpdateAsync(setting);
        return ToDto(setting);
    }

    public async Task DeleteAsync(int userId)
    {
        var setting = await repository.GetByUserIdAsync(userId)
            ?? throw new KeyNotFoundException($"No settings found for user ID: {userId}");

        await repository.RemoveAsync(setting);
    }

    private static SettingDto ToDto(Setting setting)
    {
        return new SettingDto
        {
            CurrencyCode = setting.CurrencyCode,
            LanguageCode = setting.LanguageCode
        };
    }
}