using HisabDo.Application.DTOs;
using HisabDo.Application.Repositories;
using HisabDo.Domain.Constants;
using HisabDo.Domain.Entities;

namespace HisabDo.Application.Services;

public class AuthService(
    IUserRepository repository,
    ICategoryRepository categoryRepository,
    ISettingRepository settingRepository,
    IBackupRepository backupRepository,
    ITransactionRepository transactionRepository,
    IPasswordHasher passwordHasher,
    ITokenService tokenService) : IAuthService
{
    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        PasswordPolicy.Validate(dto.Password);

        var email = dto.Email.ToLowerInvariant();

        if (await repository.EmailExistsAsync(email))
        {
            throw new InvalidOperationException($"An account with the email '{email}' already exists.");
        }

        var user = new User
        {
            FullName = dto.FullName,
            BusinessName = dto.BusinessName,
            Email = email,
            Phone = dto.Phone,
            PasswordHash = passwordHasher.Hash(dto.Password),
            Role = Roles.User,
            CurrencyCode = Defaults.CurrencyCode,
            LanguageCode = Defaults.LanguageCode
        };

        await repository.AddAsync(user);

        var defaultCategories = Defaults.DefaultCategories
            .Select(name => new Category
        {
            UserId = user.Id,
            Name = name,
            IsDefault = true
        });

        try
        {
            await categoryRepository.AddRangeAsync(defaultCategories);
            await settingRepository.AddOrUpdateAsync(new Setting { UserId = user.Id });
        }
        catch
        {
            await backupRepository.ClearAllAsync(user.Id);
            await repository.DeleteAsync(user.Id);
            throw new InvalidOperationException("Failed to create account defaults. Please try again.");
        }

        return ToAuthResponse(user);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await repository.GetByEmailAsync(dto.Email)
            ?? throw new UnauthorizedAccessException("Invalid email or password.");

        if (user.IsDeleted)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        if (!passwordHasher.Verify(dto.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        return ToAuthResponse(user);
    }

    public async Task<UserProfileDto> GetProfileAsync(int userId)
    {
        var user = await repository.GetByIdAsync(userId)
            ?? throw new KeyNotFoundException("User not found.");

        return ToProfile(user);
    }

    public async Task<UserProfileDto> UpdateProfileAsync(int userId, UpdateProfileDto dto)
    {
        var user = await repository.GetByIdAsync(userId)
            ?? throw new KeyNotFoundException("User not found.");

        user.FullName = dto.FullName;
        user.BusinessName = dto.BusinessName ?? string.Empty;
        user.Phone = dto.Phone ?? string.Empty;

        await repository.UpdateAsync(user);
        return ToProfile(user);
    }

    public async Task ChangePasswordAsync(int userId, ChangePasswordDto dto)
    {
        var user = await repository.GetByIdAsync(userId)
            ?? throw new KeyNotFoundException("User not found.");

        if (!passwordHasher.Verify(dto.OldPassword, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Current password is incorrect.");
        }

        if (dto.OldPassword == dto.NewPassword)
        {
            throw new InvalidOperationException("New password must be different from current password.");
        }

        PasswordPolicy.Validate(dto.NewPassword);

        user.PasswordHash = passwordHasher.Hash(dto.NewPassword);
        await repository.UpdateAsync(user);
    }

    private AuthResponseDto ToAuthResponse(User user)
    {
        var (token, expiresAt) = tokenService.CreateToken(user);

        return new AuthResponseDto
        {
            Token = token,
            ExpiresAt = expiresAt,
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role
        };
    }

    public async Task DeleteAccountAsync(int userId)
    {
        var transactions = await transactionRepository.GetAllAsync(userId, new TransactionFilterDto { Page = 1, PageSize = int.MaxValue });
        foreach (var tx in transactions.Items)
        {
            if (!string.IsNullOrEmpty(tx.AttachmentUrl))
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", tx.AttachmentUrl.TrimStart('/'));
                if (File.Exists(filePath))
                {
                    try { File.Delete(filePath); } catch { }
                }
            }
        }

        await backupRepository.ClearAllAsync(userId);
        await repository.DeleteAsync(userId);
    }

    private static UserProfileDto ToProfile(User user)
    {
        return new UserProfileDto
        {
            Id = user.Id,
            FullName = user.FullName,
            BusinessName = user.BusinessName,
            Email = user.Email,
            Phone = user.Phone,
            Role = user.Role,
            CurrencyCode = user.CurrencyCode,
            LanguageCode = user.LanguageCode,
            CreatedAt = user.CreatedAt
        };
    }
}
