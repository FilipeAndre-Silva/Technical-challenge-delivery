using System.Security.Claims;

namespace AgendaApp.API.Helpers;

public static class UserClaimsHelper
{
    public static Guid GetUserIdFromClaim(ClaimsPrincipal user)
    {
        var userIdClaim = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (Guid.TryParse(userIdClaim, out var userId))
        {
            return userId;
        }

        throw new InvalidOperationException("User ID is not a valid GUID.");
    }
}