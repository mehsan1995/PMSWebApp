using Application.DTOs;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IDepartmentService
    {
        Task<(List<DepartmentsDto> Data, int TotalCount)> GetPaginatedAsync(
  int start, int length, string searchValue, string sortColumn, string sortDirection);
        Task<DepartmentsDto> CreateAsync(DepartmentsDto createDto);
        Task<DepartmentsDto> GetByIdAsync(int Id);
        Task<IEnumerable<DepartmentsDto>> GetUserDepartmentAsync(string userId);
        Task<IEnumerable<DepartmentsDto>> GetAllAsync();
        Task<DepartmentTreeDto> GetDepartmentTreeAsync(int parentId);
        Task<DepartmentsDto> UpdateAsync(int Id, DepartmentsDto editDto);
        Task DeleteAsync(int Id);
        Task<bool> UpdateManager(UpdateDepartmentManagerDto dto);
        Task<IEnumerable<DepartmentTypeDto>> DepartmentTypes();
    }
}
