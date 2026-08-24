using HisabDo.Domain.Enums;

namespace HisabDo.Application.DTOs;

public class TransactionDto
{
    public int Id { get; set; }
    public int? CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public TransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public string Note { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? AttachmentUrl { get; set; }
}