using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.TenantProvider
{
    public class TenantProvider:ITenantProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TenantProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int GetTenantId()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user == null)
                return 0;

            var tenantIdClaim = user.FindFirst("tenantId");

            if (tenantIdClaim == null)
                return 0;

            // Try parsing the claim value to int
            if (int.TryParse(tenantIdClaim.Value, out int tenantId))
                return tenantId;

            return 0; // fallback if parsing fails
        }

    }
}
