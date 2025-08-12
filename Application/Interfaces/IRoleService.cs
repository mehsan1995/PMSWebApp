using Application.DTOs;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IRoleService
    {
        Task<(List<RoleDto> Data, int TotalCount)> GetPaginatedRolesAsync(
    int start, int length, string searchValue, string sortColumn, string sortDirection);
        Task<RoleDto> CreateAsync(RoleDto createDto);
        Task<RoleDto> GetByIdAsync(string Id);
        Task<IEnumerable<RoleDto>> GetUserWithRolesAsync(string userId);
        Task<IEnumerable<RoleDto>> GetAllAsync();
        Task<IEnumerable<RoleDto>> GetAllAsync( string searchTerm);
        Task<RoleDto> UpdateAsync(string Id, RoleDto editDto);
        Task DeleteAsync(string Id);
        Task ToggleStatusAsync(string Id,bool status);

    }
}
