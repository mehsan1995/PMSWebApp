using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ITenantService
    {
        Task<(List<TenantsDto> Data, int TotalCount)> GetPaginatedAsync(
  int start, int length, string searchValue, string sortColumn, string sortDirection);
        Task<TenantsDto> CreateAsync(TenantsDto tenantsDto);
        Task<TenantsDto> GetByIdAsync(int tenantId);
        Task<IEnumerable<TenantsDto>> GetAllAsync();
        Task<TenantsDto> UpdateAsync(int tenantId, TenantsDto tenantsDto);
        Task DeleteAsync(int tenantId);
    }
}
