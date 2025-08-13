using DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class ProjectInformations : IEntity, IAuditable, ISoftDeletable, ITenant
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public Projects Project { get; set; }
        public int? DepartmentId { get; set; }
        public Departments? Department { get; set; }
        public int? CompanyId { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public DateTime? ContractSignatureDate { get; set; }
        public DateTime? ProjectAssigningDate { get; set; }
        public string? EstimatedBudget { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? AssociatedObjectives { get; set; }

        public string? ContactProjectManagerName { get; set; }
        public string? ContactProjectManagerEmail { get; set; }
        public string? ContactProjectManagerMobile { get; set; }
        public List<Documents>? DocumentsList { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string? ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
        public int TenantId { get; set; }
    }
}
