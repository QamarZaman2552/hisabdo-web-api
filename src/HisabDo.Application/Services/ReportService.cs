using HisabDo.Application.DTOs;
using HisabDo.Application.Repositories;

namespace HisabDo.Application.Services;

public class ReportService(IReportRepository repository) : IReportService
{
    public Task<ReportSummaryDto> GetSummaryAsync(int userId, string? period = null)
    {
        if (!string.IsNullOrWhiteSpace(period))
        {
            var normalized = period.Trim().ToLowerInvariant();
            if (normalized is not ("week" or "month" or "3months" or "year"))
            {
                throw new InvalidOperationException("Invalid period. Allowed values: week, month, 3months, year.");
            }
        }

        var monthStart = period?.ToLower() switch
        {
            "week" => DateTime.UtcNow.AddDays(-7),
            "month" => new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc),
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

    public async Task<NotificationSummaryDto> GetNotificationsAsync(int userId)
    {
        var now = DateTime.UtcNow;
        var todayStart = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc);
        var todayEnd = todayStart.AddDays(1);
        var weekStart = now.AddDays(-7);

        var today = await repository.GetPeriodSummaryAsync(userId, todayStart, todayEnd);
        var week = await repository.GetPeriodSummaryAsync(userId, weekStart, todayEnd);

        return new NotificationSummaryDto
        {
            Today = today,
            ThisWeek = week
        };
    }
}