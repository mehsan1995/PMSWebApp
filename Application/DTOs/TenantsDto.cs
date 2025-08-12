using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class TenantsDto
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Tenant name is required")]
        [StringLength(100, ErrorMessage = "Tenant name can't exceed 100 characters")]
        [Display(Name = "Tenant Name")]
        public string TenantName { get; set; }
        [Required(ErrorMessage = "Tenant code is required")]
        [Display(Name = "Tenant Code")]
        public string? TenantCode { get; set; }
        public string? Description { get; set; }
    }
}
