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
    public class TenantService : ITenantService
    {
        private readonly IGenericRepository<Tenants> _tenantRepository;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _dbContext;
        public TenantService(
            IMapper mapper,
            ApplicationDbContext applicationDbContext)
        {
            _tenantRepository = new GenericRepository<Tenants>(applicationDbContext);
            _mapper = mapper;
            _dbContext = applicationDbContext;
        }
        public async Task<(List<TenantsDto> Data, int TotalCount)> GetPaginatedAsync(
    int start, int length, string searchValue, string sortColumn, string sortDirection)
        {
            var query = _dbContext.Tenants.AsQueryable();

            // Optional: search filter
            if (!string.IsNullOrWhiteSpace(searchValue))
            {
                query = query.Where(x =>
                    x.TenantName.Contains(searchValue) ||
                    x.Description.Contains(searchValue));
            }

            // Get total count after filtering
            var totalCount = await query.CountAsync();

            // Sorting (safe fallback to Name)
            query = sortColumn switch
            {
                "name" => sortDirection == "asc" ? query.OrderBy(x => x.TenantName) : query.OrderByDescending(x => x.TenantName),
                "description" => sortDirection == "asc" ? query.OrderBy(x => x.Description) : query.OrderByDescending(x => x.Description),
                _ => query.OrderBy(x => x.TenantName)
            };

            // Paging
            var pagedData = await query.Skip(start).Take(length).ToListAsync();

            // Mapping (if using DTOs)
            var data = pagedData.Select(x => new TenantsDto
            {
                Id = x.Id,
                TenantName = x.TenantName,
                Description = x.Description,
                TenantCode=x.TenantCode

            }).ToList();

            return (data, totalCount);
        }

        public async Task<TenantsDto> CreateAsync(TenantsDto entityDto)
        {
            try
            {
                var entity = _mapper.Map<Tenants>(entityDto);
                return _mapper.Map<TenantsDto>(await _tenantRepository.AddAsync(entity));

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task DeleteAsync(int Id)
        {
            try
            {
                var entity = await _tenantRepository.GetByIdAsync(Id);
                _tenantRepository.Delete(entity);
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<IEnumerable<TenantsDto>> GetAllAsync()
        {
            try
            {
                var entities = await _tenantRepository.GetAllAsync();
                return _mapper.Map<IEnumerable<TenantsDto>>(entities);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<TenantsDto> GetByIdAsync(int Id)
        {
            var entity = await _tenantRepository.GetByIdAsync(Id);
            return _mapper.Map<TenantsDto>(entity);
        }

        public async Task<TenantsDto> UpdateAsync(int Id, TenantsDto ediDto)
        {
            var existingEntity = await _tenantRepository.GetByIdAsync(Id);
            if (existingEntity == null)
                throw new Exception("Tenant not found");
            _mapper.Map(ediDto, existingEntity);
            await _tenantRepository.UpdateAsync(existingEntity);

            return _mapper.Map<TenantsDto>(existingEntity);
        }

        
    }
}
