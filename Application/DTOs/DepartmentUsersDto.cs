using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class DepartmentUsersDto
    {
        public int? Id { get; set; }
        public string UserId { get; set; }
        public string RoleId { get; set; }
        public int DepartmentId { get; set; }
        public string? UserName { get; set; }
    }

    public class DepartmentUsersListDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int DepartmentId { get; set; }
        public string? UserName { get; set; }
        public bool IsManager { get; set; }
        public string? Managertitle { get; set; }
    }
}
