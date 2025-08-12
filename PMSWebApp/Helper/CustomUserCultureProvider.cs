using Application.Interfaces;
using Microsoft.AspNetCore.Localization;
using System.Security.Claims;

namespace PMSWebApp.Helper
{
    public class CustomUserCultureProvider : IRequestCultureProvider
    {
        private readonly ISettingsService _settingsService;

        public CustomUserCultureProvider(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public async Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            var user = httpContext.User;
            if (!user.Identity.IsAuthenticated)
                return null; // fallback to other providers

            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return null;

            var settings = await _settingsService.GetByIdAsync(userId);
            if (settings == null || string.IsNullOrEmpty(settings.Language))
                return null;

            return new ProviderCultureResult(settings.Language, settings.Language);
        }
    }

}
