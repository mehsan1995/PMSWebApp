using Application.Implementation;
using Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {

        IUserService UserService { get; }
        IRoleService RoleService { get; }
        IDepartmentService DepartmentService { get; }
        IDepartmentTypes DepartmentTypes { get; }
        IPermissionsService PermissionService { get; }
        IDepartmentUsersService DepartmentUsersService { get; }
        ITenantService TenantService { get; }
        ISettingsService SettingsService { get; }
        IProjectService ProjectService { get; }
        Task<int> CompleteAsync();
    }
}
