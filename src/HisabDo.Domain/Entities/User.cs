using HisabDo.Domain.Common;

namespace HisabDo.Domain.Entities;

public class User : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string BusinessName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "User";
    public string CurrencyCode { get; set; } = "PKR";
    public string LanguageCode { get; set; } = "en";

    public List<Customer> Customers { get; set; } = new List<Customer>();
    public List<Category> Categories { get; set; } = new List<Category>();
    public List<Transaction> Transactions { get; set; } = new List<Transaction>();
    public Setting? Setting { get; set; }
}