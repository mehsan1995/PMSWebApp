using DAL.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{


    public class ApplicationUser : IdentityUser,ITenant, IAuditable, ISoftDeletable
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NationalID { get; set; }
        public string EmployeeID { get; set; }
        public string JobTitle { get; set; }
        public string PhoneNumber { get; set; }
        public int DepartmentsId { get; set; }
        public Departments Departments { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string? ModifiedBy { get; set; }
        public int TenantId { get; set; }
    }
}