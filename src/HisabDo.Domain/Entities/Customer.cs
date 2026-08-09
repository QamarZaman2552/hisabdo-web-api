using HisabDo.Domain.Common;

namespace HisabDo.Domain.Entities;

public class Customer : BaseEntity
{
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;

    public User? User { get; set; }
    public List<Transaction> Transactions { get; set; } = new List<Transaction>();
}