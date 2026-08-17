using HisabDo.Application.DTOs;

namespace HisabDo.Application.Services;

public interface IReportService
{
    Task<ReportSummaryDto> GetSummaryAsync(int userId);
    Task<List<CategoryReportDto>> GetCategoryBreakdownAsync(int userId);
}