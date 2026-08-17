using HisabDo.Application.DTOs;
using HisabDo.Application.Repositories;

namespace HisabDo.Application.Services;

public class ReportService(IReportRepository repository) : IReportService
{
    public Task<ReportSummaryDto> GetSummaryAsync(int userId)
    {
        var monthStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        return repository.GetSummaryAsync(userId, monthStart);
    }

    public Task<List<CategoryReportDto>> GetCategoryBreakdownAsync(int userId)
    {
        return repository.GetCategoryBreakdownAsync(userId);
    }
}