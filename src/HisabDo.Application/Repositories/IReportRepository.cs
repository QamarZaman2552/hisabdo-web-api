using HisabDo.Application.DTOs;

namespace HisabDo.Application.Repositories;

public interface IReportRepository
{
    Task<ReportSummaryDto> GetSummaryAsync(int userId, DateTime monthStart);
    Task<List<CategoryReportDto>> GetCategoryBreakdownAsync(int userId);
}