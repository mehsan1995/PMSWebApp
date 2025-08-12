using Application.DTOs;

namespace PMSWebApp.Models
{
    public class DepartmentUsersViewModel
    {
        public int DepartmentId { get; set; }
        public List<DepartmentUsersListDto> Users { get; set; }= new List<DepartmentUsersListDto>();
    }
}
