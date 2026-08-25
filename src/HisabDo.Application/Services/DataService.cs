using HisabDo.Application.DTOs;
using HisabDo.Application.Repositories;

namespace HisabDo.Application.Services;

public interface IDataService
{
    Task<BackupFileDto> ExportAsync(int userId);
    Task<RestoreResultDto> RestoreAsync(int userId, BackupFileDto data, bool replaceExisting);
    Task ClearAllAsync(int userId);
}

public class DataService(IBackupRepository repository) : IDataService
{
    public Task<BackupFileDto> ExportAsync(int userId) => repository.ExportAsync(userId);

    public Task<RestoreResultDto> RestoreAsync(int userId, BackupFileDto data, bool replaceExisting)
        => repository.RestoreAsync(userId, data, replaceExisting);

    public Task ClearAllAsync(int userId) => repository.ClearAllAsync(userId);
}
