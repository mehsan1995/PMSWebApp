using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IProjectService
    {
        Task<(List<ProjectsDto> Data, int TotalCount)> GetPaginatedRolesAsync(
    int start, int length, string searchValue, string sortColumn, string sortDirection);
        Task<ProjectsDto> CreateAsync(ProjectsDto createDto);
        Task<ProjectsDto> GetByIdAsync(int Id);
        Task<ProjectsDto> UpdateAsync(int Id, ProjectsDto editDto);


        Task<ProjectInformationsDto> CreateProjectInformationAsync(ProjectInformationsDto createDto);
        Task<ProjectInformationsDto> GetProjectInformationByIdAsync(int Id);
        Task<ProjectInformationsDto> UpdateProjectInformationAsync(int Id, ProjectInformationsDto editDto);
        Task DeleteProjectInformationAsync(int Id);
        Task<DocumentsDto> UploadDocuments(DocumentsDto editDto);


        Task DeleteAsync(int Id);
        Task ToggleStatusAsync(int Id, bool status);
    }
}
