using HisabDo.API.Extensions;
using HisabDo.Application.DTOs;
using HisabDo.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HisabDo.API.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class ReportsController(IReportService reportService) : ControllerBase
{
    [HttpGet("summary")]
    public async Task<ActionResult<ReportSummaryDto>> GetSummary([FromQuery] string? period = null)
    {
        return Ok(await reportService.GetSummaryAsync(User.GetUserId(), period));
    }

    [HttpGet("by-category")]
    public async Task<ActionResult<List<CategoryReportDto>>> GetCategoryBreakdown()
    {
        return Ok(await reportService.GetCategoryBreakdownAsync(User.GetUserId()));
    }

    [HttpGet("notifications")]
    public async Task<ActionResult<NotificationSummaryDto>> GetNotifications()
    {
        return Ok(await reportService.GetNotificationsAsync(User.GetUserId()));
    }
}