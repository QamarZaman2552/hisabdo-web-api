using HisabDo.Application.DTOs;
using HisabDo.Application.Repositories;
using HisabDo.Domain.Enums;
using HisabDo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HisabDo.Infrastructure.Repositories;

public class ReportRepository(HisabDoDbContext context) : IReportRepository
{
    public async Task<ReportSummaryDto> GetSummaryAsync(int userId, DateTime monthStart)
    {
        var totals = await context.Transactions
            .Where(t => t.UserId == userId && !t.IsDeleted)
            .GroupBy(t => 1)
            .Select(g => new
            {
                Count = g.Count(),
                Receivable = g.Where(t => t.Type == TransactionType.Receivable).Sum(t => t.Amount),
                Payable = g.Where(t => t.Type == TransactionType.Payable).Sum(t => t.Amount)
            })
            .FirstOrDefaultAsync();

        var monthTotals = await context.Transactions
            .Where(t => t.UserId == userId && !t.IsDeleted && t.TransactionDate >= monthStart)
            .GroupBy(t => 1)
            .Select(g => new
            {
                Count = g.Count(),
                Received = g.Where(t => t.Type == TransactionType.Receivable).Sum(t => t.Amount),
                Paid = g.Where(t => t.Type == TransactionType.Payable).Sum(t => t.Amount)
            })
            .FirstOrDefaultAsync();

        return new ReportSummaryDto
        {
            TotalCustomers = await context.Customers.CountAsync(c => c.UserId == userId && !c.IsDeleted),
            TotalCategories = await context.Categories.CountAsync(c => c.UserId == userId && !c.IsDeleted),
            TotalTransactions = totals?.Count ?? 0,
            TotalReceivable = totals?.Receivable ?? 0,
            TotalPayable = totals?.Payable ?? 0,
            Balance = (totals?.Receivable ?? 0) - (totals?.Payable ?? 0),
            ThisMonthTransactions = monthTotals?.Count ?? 0,
            ThisMonthReceived = monthTotals?.Received ?? 0,
            ThisMonthPaid = monthTotals?.Paid ?? 0
        };
    }

    public async Task<List<CategoryReportDto>> GetCategoryBreakdownAsync(int userId)
    {
        return await context.Transactions
            .Where(t => t.UserId == userId && !t.IsDeleted)
            .GroupBy(t => new { t.CategoryId, t.Category!.Name })
            .Select(g => new CategoryReportDto
            {
                CategoryId = g.Key.CategoryId,
                CategoryName = g.Key.Name,
                TransactionCount = g.Count(),
                ReceivableTotal = g.Where(t => t.Type == TransactionType.Receivable).Sum(t => t.Amount),
                PayableTotal = g.Where(t => t.Type == TransactionType.Payable).Sum(t => t.Amount)
            })
            .OrderByDescending(c => c.TransactionCount)
            .ToListAsync();
    }

    public async Task<PeriodSummaryDto> GetPeriodSummaryAsync(int userId, DateTime from, DateTime to)
    {
        var summary = await context.Transactions
            .Where(t => t.UserId == userId && !t.IsDeleted && t.TransactionDate >= from && t.TransactionDate < to)
            .GroupBy(t => 1)
            .Select(g => new
            {
                Count = g.Count(),
                Receivable = g.Where(t => t.Type == TransactionType.Receivable).Sum(t => t.Amount),
                Payable = g.Where(t => t.Type == TransactionType.Payable).Sum(t => t.Amount)
            })
            .FirstOrDefaultAsync();

        return new PeriodSummaryDto
        {
            Transactions = summary?.Count ?? 0,
            Receivable = summary?.Receivable ?? 0,
            Payable = summary?.Payable ?? 0
        };
    }
}