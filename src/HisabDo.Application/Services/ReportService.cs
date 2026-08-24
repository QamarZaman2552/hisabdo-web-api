using HisabDo.Application.DTOs;
using HisabDo.Application.Repositories;

namespace HisabDo.Application.Services;

public class ReportService(IReportRepository repository) : IReportService
{
    public Task<ReportSummaryDto> GetSummaryAsync(int userId, string? period = null)
    {
        var monthStart = period?.ToLower() switch
        {
            "week" => DateTime.UtcNow.AddDays(-7),
            "3months" => DateTime.UtcNow.AddMonths(-3),
            "year" => DateTime.UtcNow.AddYears(-1),
            _ => new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        return repository.GetSummaryAsync(userId, monthStart);
    }

    public Task<List<CategoryReportDto>> GetCategoryBreakdownAsync(int userId)
    {
        return repository.GetCategoryBreakdownAsync(userId);
    }
}