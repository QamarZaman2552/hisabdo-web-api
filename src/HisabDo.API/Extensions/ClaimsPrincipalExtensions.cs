using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace HisabDo.API.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(JwtRegisteredClaimNames.Sub);

        return value != null && int.TryParse(value, out var userId) ? userId : 0;
    }
}