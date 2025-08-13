using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class ProjectsDto
    {
        public int? Id { get; set; }
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
    }
}
