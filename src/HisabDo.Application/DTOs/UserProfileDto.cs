namespace HisabDo.Application.DTOs;

public class UserProfileDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string BusinessName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string CurrencyCode { get; set; } = string.Empty;
    public string LanguageCode { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
