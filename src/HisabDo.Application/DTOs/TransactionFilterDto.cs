using System.ComponentModel.DataAnnotations;
using HisabDo.Domain.Enums;

namespace HisabDo.Application.DTOs;

public class TransactionFilterDto
{
    public TransactionType? Type { get; set; }
    public int? CustomerId { get; set; }
    public int? CategoryId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    [StringLength(100)]
    public string? Search { get; set; }
    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;
    [Range(1, 100)]
    public int PageSize { get; set; } = 50;
}