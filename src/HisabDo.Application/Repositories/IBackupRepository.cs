using HisabDo.Application.DTOs;

namespace HisabDo.Application.Repositories;

public interface IBackupRepository
{
    Task<BackupFileDto> ExportAsync(int userId);
    Task<RestoreResultDto> RestoreAsync(int userId, BackupFileDto data, bool replaceExisting);
    Task ClearAllAsync(int userId);
}
