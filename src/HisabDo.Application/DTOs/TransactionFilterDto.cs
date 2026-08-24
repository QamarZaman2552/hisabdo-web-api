using HisabDo.Domain.Enums;

namespace HisabDo.Application.DTOs;

public class TransactionFilterDto
{
    public TransactionType? Type { get; set; }
    public int? CustomerId { get; set; }
    public int? CategoryId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? Search { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}