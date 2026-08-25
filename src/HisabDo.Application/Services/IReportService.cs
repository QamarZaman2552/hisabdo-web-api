using HisabDo.Application.DTOs;

namespace HisabDo.Application.Services;

public interface IReportService
{
    Task<ReportSummaryDto> GetSummaryAsync(int userId, string? period = null);
    Task<List<CategoryReportDto>> GetCategoryBreakdownAsync(int userId);
    Task<NotificationSummaryDto> GetNotificationsAsync(int userId);
}