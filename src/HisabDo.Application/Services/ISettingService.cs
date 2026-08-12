using HisabDo.Application.DTOs;

namespace HisabDo.Application.Services;

public interface ISettingService
{
    Task<SettingDto?> GetAsync(int userId);
    Task<SettingDto> UpdateAsync(int userId, UpdateSettingDto dto);
    Task DeleteAsync(int userId);
}