namespace HisabDo.Application.DTOs;

public class PasswordPolicySettings
{
    public int MinLength { get; set; } = 8;
    public int MaxLength { get; set; } = 64;
    public bool RequireUppercase { get; set; } = true;
    public bool RequireLowercase { get; set; } = true;
    public bool RequireDigit { get; set; } = true;
    public bool RequireSpecialChar { get; set; } = true;
}
