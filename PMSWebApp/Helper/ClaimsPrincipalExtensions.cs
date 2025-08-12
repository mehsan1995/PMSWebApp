using System.Security.Claims;

namespace PMSWebApp.Helper
{
    public static class ClaimsPrincipalExtensions
    {
        public static bool HasPermission(this ClaimsPrincipal user, string permission)
        {
            if (user == null || string.IsNullOrWhiteSpace(permission))
                return false;

            var claim = user.FindFirst("PermissionIds")?.Value;
            if (string.IsNullOrWhiteSpace(claim))
                return false;

            var permissions = claim.Split(',', StringSplitOptions.RemoveEmptyEntries);
            return permissions.Contains(permission, StringComparer.OrdinalIgnoreCase);
        }
    }

}
