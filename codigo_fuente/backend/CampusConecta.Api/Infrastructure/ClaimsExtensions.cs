using System.Security.Claims;

namespace CampusConecta.Api.Infrastructure;

public static class ClaimsExtensions
{
    public static Guid UserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(value, out var id) ? id : throw new UnauthorizedAccessException("Identidad inválida.");
    }
}
