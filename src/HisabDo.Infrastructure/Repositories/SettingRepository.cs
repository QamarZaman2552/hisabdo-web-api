using HisabDo.Application.Repositories;
using HisabDo.Domain.Entities;
using HisabDo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HisabDo.Infrastructure.Repositories;

public class SettingRepository(HisabDoDbContext context) : ISettingRepository
{
    public async Task<Setting?> GetByUserIdAsync(int userId)
    {
        return await context.Settings
            .FirstOrDefaultAsync(s => s.UserId == userId);
    }

    public async Task<Setting> AddOrUpdateAsync(Setting setting)
    {
        if (setting.Id == 0)
        {
            context.Settings.Add(setting);
        }
        else
        {
            context.Settings.Update(setting);
        }

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new InvalidOperationException("A concurrency error occurred. Please try again.");
        }
        return setting;
    }

    public async Task RemoveAsync(Setting setting)
    {
        setting.IsDeleted = true;
        context.Settings.Update(setting);
        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new InvalidOperationException("A concurrency error occurred. Please try again.");
        }
    }
}
