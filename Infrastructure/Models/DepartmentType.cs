using DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class DepartmentType : IEntity, ITenant, IAuditable, ISoftDeletable
    {
        public int Id {get;set;}
        public string Name {get;set; }
        public int TenantId {get;set;}
        public DateTime CreatedAt {get;set;}
        public string? CreatedBy {get;set;}
        public DateTime? ModifiedAt {get;set;}
        public string? ModifiedBy {get;set;}
        public bool IsDeleted {get;set;}
    }
}
