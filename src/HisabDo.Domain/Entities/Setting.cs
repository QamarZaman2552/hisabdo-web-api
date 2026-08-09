namespace HisabDo.Domain.Entities;

public class Setting
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string CurrencyCode { get; set; } = "PKR";
    public string LanguageCode { get; set; } = "en";

    public User? User { get; set; }
}