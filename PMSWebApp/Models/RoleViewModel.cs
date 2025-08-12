using Application.DTOs;

namespace PMSWebApp.Models
{
    public class RoleViewModel
    {
        public RoleDto Role { get; set; }
        public List<RoleDto> Roles { get; set; } = new List<RoleDto>();
    }
}
