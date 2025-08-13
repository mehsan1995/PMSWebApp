using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class DocumentsDto
    {
        public int? Id { get; set; }
        public int? ProjectId { get; set; }
        public string? DocumentPath { get; set; }
    }
}
