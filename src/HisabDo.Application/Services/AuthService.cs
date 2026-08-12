using HisabDo.Application.DTOs;
using HisabDo.Application.Repositories;
using HisabDo.Domain.Entities;

namespace HisabDo.Application.Services;

public class AuthService(
    IUserRepository repository,
    IPasswordHasher passwordHasher,
    ITokenService tokenService) : IAuthService
{
    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        if (await repository.EmailExistsAsync(dto.Email))
        {
            throw new InvalidOperationException($"An account with the email '{dto.Email}' already exists.");
        }

        var user = new User
        {
            FullName = dto.FullName,
            BusinessName = dto.BusinessName,
            Email = dto.Email,
            Phone = dto.Phone,
            PasswordHash = passwordHasher.Hash(dto.Password),
            Role = "User"
        };

        await repository.AddAsync(user);
        return ToAuthResponse(user);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await repository.GetByEmailAsync(dto.Email)
            ?? throw new UnauthorizedAccessException("Invalid email or password.");

        if (!passwordHasher.Verify(dto.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        return ToAuthResponse(user);
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
}
