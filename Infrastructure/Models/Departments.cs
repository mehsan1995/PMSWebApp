using DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Departments: IEntity, ITenant, IAuditable, ISoftDeletable
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public int? ParentId { get; set; }
        public Departments? Parent { get; set; }
        public string? ManagerId { get; set; }
        public string? ManagerTitle { get; set; } 
        public int? DepTypeId { get; set; }
        public DepartmentType? DepartmentType { get; set; }
        public int? GeneralOrder { get; set; }
        public int? ParentOrder { get; set; }
        public string? DepartmentCode { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string? ModifiedBy { get; set; }
        public int TenantId { get; set; }
        public ICollection<ApplicationUser> Users { get; set; }
    }
}
