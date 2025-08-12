using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using DAL;
using DAL.GenericRepository;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Implementation
{

    public class RoleService : IRoleService
    {
        private readonly IGenericRepository<ApplicationRole> _roleRepository;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _dbContext;
        public RoleService(
            IMapper mapper,
            ApplicationDbContext applicationDbContext)
        {
            _roleRepository = new GenericRepository<ApplicationRole>(applicationDbContext);
            _mapper = mapper;
            _dbContext = applicationDbContext;
        }
        public async Task<(List<RoleDto> Data, int TotalCount)> GetPaginatedRolesAsync(
    int start, int length, string searchValue, string sortColumn, string sortDirection)
        {
            var query = _dbContext.Roles.AsQueryable();

            // Optional: search filter
            if (!string.IsNullOrWhiteSpace(searchValue))
            {
                query = query.Where(x =>
                    x.Name.Contains(searchValue) ||
                    x.Description.Contains(searchValue));
            }

            // Get total count after filtering
            var totalCount = await query.CountAsync();

            // Sorting (safe fallback to Name)
            query = sortColumn switch
            {
                "name" => sortDirection == "asc" ? query.OrderBy(x => x.Name) : query.OrderByDescending(x => x.Name),
                "description" => sortDirection == "asc" ? query.OrderBy(x => x.Description) : query.OrderByDescending(x => x.Description),
                _ => query.OrderBy(x => x.Name)
            };

            // Paging
            var pagedData = await query.Skip(start).Take(length).ToListAsync();

            // Mapping (if using DTOs)
            var data = pagedData.Select(x => new RoleDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                IsActive=x.IsActive

            }).ToList();

            return (data, totalCount);
        }

        public async Task<RoleDto> CreateAsync(RoleDto roleDto)
        {
            try
            {
                var role = _mapper.Map<ApplicationRole>(roleDto);
                role.Id = Guid.NewGuid().ToString();
                role.NormalizedName = role.Name.ToUpperInvariant();
                return _mapper.Map<RoleDto>(await _roleRepository.AddAsync(role));

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task DeleteAsync(string Id)
        {
            try
            {
                var role = await _roleRepository.GetByIdAsync(Id);
                _roleRepository.Delete(role);
            }
            catch (Exception)
            {

                throw;
            }
           
        }

        public async Task<IEnumerable<RoleDto>> GetAllAsync()
        {
            try
            {
                var roles = await _roleRepository.GetAllAsync();
                return _mapper.Map<IEnumerable<RoleDto>>(roles);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<IEnumerable<RoleDto>> GetAllAsync(string searchTerm)
        {
            try
            {
                var query = from role in _dbContext.Roles
                            select new RoleDto
                            {
                                Id = role.Id,
                                Name=role.Name
                            };

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    var term = searchTerm.ToLower();
                    query = query.Where(u =>
                        u.Name.ToLower().Contains(term));
                }

                return await query.ToListAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<RoleDto> GetByIdAsync(string Id)
        {
            var role = await _roleRepository.GetByIdAsync(Id);
            return _mapper.Map<RoleDto>(role);
        }

        public async Task<RoleDto> UpdateAsync(string Id, RoleDto ediDto)
        {
            var existingRole = await _roleRepository.GetByIdAsync(Id);
            if (existingRole == null)
                throw new Exception("Role not found");

            existingRole.NormalizedName = ediDto.Name.ToUpperInvariant();

            _mapper.Map(ediDto, existingRole);
            await _roleRepository.UpdateAsync(existingRole);

            return _mapper.Map<RoleDto>(existingRole);
        }

        public async Task ToggleStatusAsync(string Id, bool status)
        {
            try
            {
                var role = await _roleRepository.GetByIdAsync(Id);
                role.IsActive = status;
                await _roleRepository.UpdateAsync(role);
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        public async Task<IEnumerable<RoleDto>> GetUserWithRolesAsync(string userId)
        {
            var roles = await (from role in _dbContext.Roles
                               join userRole in _dbContext.UserRoles on role.Id equals userRole.RoleId
                               where userRole.UserId == userId
                               select role).ToListAsync();

            return _mapper.Map<IEnumerable<RoleDto>>(roles);

        }

        //public async Task<IdentityRole> CreateRoleAsync(string roleName)
        //{
        //    var roleExist = await _roleManager.RoleExistsAsync(roleName);
        //    if (roleExist)
        //        throw new Exception("Role already exists");

        //    var role = new IdentityRole(roleName);
        //    var result = await _roleManager.CreateAsync(role);

        //    if (result.Succeeded)
        //        return role;

        //    throw new Exception("Error creating role: " + string.Join(", ", result.Errors.Select(e => e.Description)));
        //}

        //public async Task<IdentityRole> GetRoleByIdAsync(string roleId)
        //{
        //    return await _roleManager.FindByIdAsync(roleId);
        //}

        //public async Task<IEnumerable<IdentityRole>> GetAllRolesAsync()
        //{
        //    return _roleManager.Roles.ToList();
        //}

        //public async Task<IdentityRole> UpdateRoleAsync(string roleId, string newRoleName)
        //{
        //    var role = await _roleManager.FindByIdAsync(roleId);
        //    if (role == null) throw new Exception("Role not found");

        //    role.Name = newRoleName;
        //    var result = await _roleManager.UpdateAsync(role);

        //    if (result.Succeeded)
        //        return role;

        //    throw new Exception("Error updating role: " + string.Join(", ", result.Errors.Select(e => e.Description)));
        //}

        //public async Task DeleteRoleAsync(string roleId)
        //{
        //    var role = await _roleManager.FindByIdAsync(roleId);
        //    if (role == null) throw new Exception("Role not found");

        //    var result = await _roleManager.DeleteAsync(role);

        //    if (!result.Succeeded)
        //        throw new Exception("Error deleting role: " + string.Join(", ", result.Errors.Select(e => e.Description)));
        //}
    }

}
