using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class PermissionDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? ParentName { get; set; }
        public bool IsChecked { get; set; } = false; 
    }
}
