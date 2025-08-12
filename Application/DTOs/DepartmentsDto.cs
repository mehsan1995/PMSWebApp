using DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    
    public class DepartmentsDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required")]
        [Display(Name = "Department Name")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Department Name")]
        public int? ParentId { get; set; }
        [Required(ErrorMessage = "Manager Title is required")]
        [Display(Name = "Manager Title")]
        public string? ManagerTitle { get; set; }
        [Required(ErrorMessage = "Department Type is required")]
        [Display(Name = "Department Type")]
        public int? DepTypeId { get; set; }
        [Required(ErrorMessage = "General Order is required")]
        [Display(Name = "General Order")]
        public int? GeneralOrder { get; set; }
        [Required(ErrorMessage = "Parent Order is required")]
        [Display(Name = "Parent Order")]
        public int? ParentOrder { get; set; }
        [Required(ErrorMessage = "Department Code is required")]
        [Display(Name = "Department Code")]
        public string? DepartmentCode { get; set; }
        [Required(ErrorMessage = "Signature is required")]
        [Display(Name = "Signature")]
        public string? Signature { get; set; }
    }
    public class DepartmentTreeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<DepartmentTreeDto> ChildDeparts { get; set; } = new List<DepartmentTreeDto>();

    }
    public class UpdateDepartmentManagerDto
    {
        public int DepartmentId { get; set; }
        public string UserId { get; set; }
    }
}
