using Application.Implementation;
using Application.Interfaces;
using AutoMapper;
using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public IUserService UserService { get; }
        public IRoleService RoleService { get; }
        public IDepartmentService DepartmentService { get; }
        public IDepartmentTypes DepartmentTypes { get; }
        public IPermissionsService PermissionService { get; }
        public IDepartmentUsersService DepartmentUsersService { get; }
        public ISettingsService SettingsService { get; }

        public ITenantService TenantService { get; }

        public UnitOfWork(ApplicationDbContext context, IUserService userService, IRoleService roleService,
               IDepartmentService departmentService, IPermissionsService permissionService,
               ITenantService tenantService, IDepartmentTypes departmentTypes, IDepartmentUsersService departmentUsersService,
               ISettingsService settingsService)
        {
            _context = context;
            UserService = userService;
            RoleService = roleService;
            DepartmentService = departmentService;
            PermissionService = permissionService;
            TenantService = tenantService;
            DepartmentTypes = departmentTypes;
            DepartmentUsersService = departmentUsersService;
            SettingsService = settingsService;
        }

        public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();

        public void Dispose() => _context.Dispose();
    }

}
