using HisabDo.Domain.Entities;

namespace HisabDo.Application.Services;

public interface ITokenService
{
    (string Token, DateTime ExpiresAt) CreateToken(User user);
}