using DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Projects : IEntity, IAuditable, ISoftDeletable, ITenant
    {
        public int Id { get; set; }
        public string ProjectID { get; set; }
        public string ProjectName { get; set; }
        public string? Description { get; set; }
        public int? PortfolioId { get; set; }
        public int? ProgramId { get; set; }
        public string? UserId { get; set; }
        public string? ProjectDuration { get; set; }
        public int? StatusId { get; set; }
        public int? StageId { get; set; }
        public int? Priority { get; set; }
        public int? Weight { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string? ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
        int ITenant.TenantId { get; set; }
    }
}
