using HisabDo.Application.DTOs;

namespace HisabDo.Application.Services;

public static class PasswordPolicy
{
    private static PasswordPolicySettings _settings = new();

    public static void Configure(PasswordPolicySettings settings)
    {
        _settings = settings;
    }

    public static int MinLength => _settings.MinLength;
    public static int MaxLength => _settings.MaxLength;

    public static void Validate(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException("Password is required.");
        }

        if (password.Length < _settings.MinLength || password.Length > _settings.MaxLength)
        {
            throw new InvalidOperationException($"Password must be between {_settings.MinLength} and {_settings.MaxLength} characters.");
        }

        if (_settings.RequireUppercase && !password.Any(char.IsUpper))
        {
            throw new InvalidOperationException("Password must contain at least one uppercase letter.");
        }

        if (_settings.RequireLowercase && !password.Any(char.IsLower))
        {
            throw new InvalidOperationException("Password must contain at least one lowercase letter.");
        }

        if (_settings.RequireDigit && !password.Any(char.IsDigit))
        {
            throw new InvalidOperationException("Password must contain at least one digit.");
        }

        if (_settings.RequireSpecialChar && !password.Any(c => !char.IsLetterOrDigit(c)))
        {
            throw new InvalidOperationException("Password must contain at least one special character.");
        }
    }
}
