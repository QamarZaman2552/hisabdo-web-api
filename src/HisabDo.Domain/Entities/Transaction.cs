using HisabDo.Domain.Common;
using HisabDo.Domain.Enums;

namespace HisabDo.Domain.Entities;

public class Transaction : BaseEntity
{
    public int UserId { get; set; }
    public int? CustomerId { get; set; }
    public int CategoryId { get; set; }
    public TransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public string Note { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    public string? AttachmentUrl { get; set; }

    public User? User { get; set; }
    public Customer? Customer { get; set; }
    public Category? Category { get; set; }
}