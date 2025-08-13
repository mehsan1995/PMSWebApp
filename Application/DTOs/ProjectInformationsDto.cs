using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class ProjectInformationsDto
    {
        public int? Id { get; set; }
        public int ProjectId { get; set; }
        public int? DepartmentId { get; set; }
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
        public List<DocumentsDto>? DocumentsList { get; set; }
    }
}
