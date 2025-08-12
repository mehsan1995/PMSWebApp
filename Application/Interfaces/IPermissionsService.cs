using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IPermissionsService
    {
        Task<IEnumerable<PermissionModulesDto>> GetAllAsync(string roleId);
        Task<IEnumerable<PermissionDto>> GetPermissionsByRoleAsync(string roleId);
        Task SaveRolePermissionsAsync(string roleId, List<int> permissionModulesDtos);
    }
}
