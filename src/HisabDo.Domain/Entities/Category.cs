using HisabDo.Domain.Common;

namespace HisabDo.Domain.Entities;

public class Category : BaseEntity
{
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsDefault { get; set; }

    public User? User { get; set; }
    public List<Transaction> Transactions { get; set; } = new List<Transaction>();
}