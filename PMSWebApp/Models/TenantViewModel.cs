using Application.DTOs;

namespace PMSWebApp.Models
{
    public class TenantViewModel
    {
        public TenantsDto Tenant { get; set; }
        public List<TenantsDto> TenantsList { get; set; } = new List<TenantsDto>();
    }
}
