using Application.DTOs;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PMSWebApp.Models
{
    public class CreateDepartmentViewModel
    {
        public int? ParentId { get; set; }
        public DepartmentsDto Department { get; set; } = new DepartmentsDto();
        public List<SelectListItem> DepartmentsType { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> DepartmentList { get; set; } = new List<SelectListItem>();
    }
    public class DepartmentViewModel:CreateDepartmentViewModel
    {
        
        public List<DepartmentsDto> Departments { get; set; } = new List<DepartmentsDto>();
        public DepartmentTreeDto DepartmentsTree { get; set; } = new DepartmentTreeDto();
    }

}
