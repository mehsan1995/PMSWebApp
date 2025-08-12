using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using DAL;
using DAL.GenericRepository;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Implementation
{
    public class PermissionsService : IPermissionsService
    {
        private readonly IGenericRepository<RolePermission> _rolePermissions;
        private readonly ApplicationDbContext _dbContext;
        public PermissionsService(ApplicationDbContext applicationDbContext)
        {
             _rolePermissions = new GenericRepository<RolePermission>(applicationDbContext);
            _dbContext = applicationDbContext;
        }
        public async Task<IEnumerable<PermissionModulesDto>> GetAllAsync(string roleId)
        {
            var grouped = await _dbContext.Permissions
                                           .Where(p => p.ParentId == null)
                                           .Select(parent => new PermissionModulesDto
                                           {
                                               Id = parent.Id,
                                               Name = parent.Name,
                                               Permissions = _dbContext.Permissions
                                                   .Where(c => c.ParentId == parent.Id)
                                                   .Select(c => new PermissionDto
                                                   {
                                                       Id = c.Id,
                                                       Name = c.Name,
                                                       IsChecked = _dbContext.RolePermissions
                                                           .Any(rp => rp.PermissionId == c.Id && rp.RoleId == roleId)
                                                   })
                                                   .ToList()
                                           })
                                           .OrderBy(m => m.Id)
                                           .ToListAsync();


            return grouped;

        }
        public async Task<IEnumerable<PermissionDto>> GetPermissionsByRoleAsync(string roleId)
        {
            var permissions = await _dbContext.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .Select(rp => new PermissionDto
                {
                    Id = rp.Permission.Id,
                    Name = rp.Permission.Name,
                    ParentName = rp.Permission.Parent != null ? rp.Permission.Parent.Name : null
                })
                .ToListAsync();

            return permissions;
        }

        public async Task SaveRolePermissionsAsync(string roleId, List<int> selectedPermissionIds)
        {
            // Get existing permissions
            var existingPermissions = await _rolePermissions.FindAsync(rp => rp.RoleId == roleId);

            // Delete unselected
            var toRemove = existingPermissions
                .Where(rp => !selectedPermissionIds.Contains(rp.PermissionId))
                .ToList();

            _rolePermissions.RemoveRange(toRemove);

            // Add newly selected
            var existingPermissionIds = existingPermissions.Select(rp => rp.PermissionId).ToList();

            var toAdd = selectedPermissionIds
                .Where(id => !existingPermissionIds.Contains(id))
                .Select(id => new RolePermission { RoleId = roleId, PermissionId = id });

            await _rolePermissions.AddRangeAsync(toAdd);
        }

    }
}
