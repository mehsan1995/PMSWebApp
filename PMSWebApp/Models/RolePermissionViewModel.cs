using Application.DTOs;

namespace PMSWebApp.Models
{
    public class RolePermissionViewModel
    {
        public string RoleId { get; set; }
        public string? RoleName { get; set; }
        public bool? IsView { get; set; }
        public List<int> SelectedPermissionIds { get; set; } = new();
        public List<PermissionModulesDto> PermissionModules { get; set; } = new List<PermissionModulesDto>();
    }
}
