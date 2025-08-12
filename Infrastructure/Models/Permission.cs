using DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Permission: IAuditable, ISoftDeletable
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int? ParentId { get; set; }
        public Permission Parent {get; set;}
        public DateTime CreatedAt { get ; set ; }
        public string? CreatedBy { get ; set ; }
        public DateTime? ModifiedAt { get ; set ; }
        public string? ModifiedBy { get ; set ; }
        public bool IsDeleted { get ; set ; }
    }

}
