using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace API.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        var userId = principal.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
                     principal.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userId, out var parsedUserId))
        {
            throw new UnauthorizedAccessException("Authenticated user id claim is missing or invalid.");
        }

        return parsedUserId;
    }
}
