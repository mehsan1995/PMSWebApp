using DAL.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace DAL.Models
{
    public class RolePermission:ITenant, IAuditable, ISoftDeletable
    {
        public string RoleId { get; set; } = null!;
        public ApplicationRole Role { get; set; } = null!;

        public int PermissionId { get; set; }
        public Permission Permission { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string? ModifiedBy { get; set; }
        public int TenantId { get; set; }
        public bool IsDeleted { get; set; }
    }

}
