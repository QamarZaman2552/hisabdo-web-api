namespace HisabDo.Application.Services;

public static class PasswordPolicy
{
    public const int MinLength = 8;
    public const int MaxLength = 64;

    public static void Validate(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException("Password is required.");
        }

        if (password.Length < MinLength || password.Length > MaxLength)
        {
            throw new InvalidOperationException($"Password must be between {MinLength} and {MaxLength} characters.");
        }

        if (!password.Any(char.IsUpper))
        {
            throw new InvalidOperationException("Password must contain at least one uppercase letter.");
        }

        if (!password.Any(char.IsLower))
        {
            throw new InvalidOperationException("Password must contain at least one lowercase letter.");
        }

        if (!password.Any(char.IsDigit))
        {
            throw new InvalidOperationException("Password must contain at least one digit.");
        }

        if (!password.Any(c => !char.IsLetterOrDigit(c)))
        {
            throw new InvalidOperationException("Password must contain at least one special character.");
        }
    }
}
