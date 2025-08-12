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
    public class DepartmentService : IDepartmentService
    {
        private readonly IGenericRepository<Departments> _departmentRepository;
        private readonly IGenericRepository<DepartmentType> _departmentTypeRepository;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _dbContext;
        public DepartmentService(
            IMapper mapper,
            ApplicationDbContext applicationDbContext)
        {
            _mapper = mapper;
            _dbContext = applicationDbContext;
            _departmentRepository = new GenericRepository<Departments>(applicationDbContext);
         
            _departmentTypeRepository = new GenericRepository<DepartmentType>(applicationDbContext); ;
        }
        public async Task<DepartmentsDto> CreateAsync(DepartmentsDto createDto)
        {
            try
            {
                var department = _mapper.Map<Departments>(createDto);
                return _mapper.Map<DepartmentsDto>(await _departmentRepository.AddAsync(department));

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
                var department = await _departmentRepository.GetByIdAsync(Id);
                if (department is not null)
                {
                     _departmentRepository.Delete(department);
                    
                }
                throw new KeyNotFoundException($"Department with Id {Id} not found.");

            }
            catch (Exception)
            {

                throw;
            }
          
        }

        public async Task<IEnumerable<DepartmentsDto>> GetAllAsync()
        {
            try
            {
                var departments = await _departmentRepository.GetAllAsync();
                return _mapper.Map<IEnumerable<DepartmentsDto>>(departments);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<DepartmentTreeDto?> GetDepartmentTreeAsync(int parentId)
        {
            var allDepartments = await _dbContext.Departments
                .Select(d => new
                {
                    d.Id,
                    d.ParentId,
                    d.Name
                })
                .ToListAsync();

            // Build a dictionary for quick lookup
            var departmentDict = allDepartments.ToDictionary(d => d.Id);

            DepartmentTreeDto? BuildTree(int id)
            {
                if (!departmentDict.TryGetValue(id, out var department))
                    return null;

                var children = allDepartments
                    .Where(d => d.ParentId == id)
                    .Select(child => BuildTree(child.Id))
                    .Where(child => child != null)
                    .ToList();

                return new DepartmentTreeDto
                {
                    Id = department.Id,
                    Name = department.Name,
                    ChildDeparts = children.Any() ? children! : null
                };
            }
            var data = BuildTree(parentId);
            return data;
        }





        public async Task<DepartmentsDto> GetByIdAsync(int Id)
        {
            try
            {
                return _mapper.Map<DepartmentsDto>(await _departmentRepository.GetByIdAsync(Id));
            }
            catch (Exception)
            {

                throw;
            }
          
        }

        public async Task<(List<DepartmentsDto> Data, int TotalCount)> GetPaginatedAsync(int start, int length, string searchValue, string sortColumn, string sortDirection)
        {
            try
            {
                var query = _dbContext.Departments.AsQueryable();

                // Optional: search filter
                if (!string.IsNullOrWhiteSpace(searchValue))
                {
                    query = query.Where(x =>
                        x.Name.Contains(searchValue) ||
                        x.DepartmentCode.Contains(searchValue));
                }

                // Get total count after filtering
                var totalCount = await query.CountAsync();

                // Sorting (safe fallback to Name)
                query = sortColumn switch
                {
                    "name" => sortDirection == "asc" ? query.OrderBy(x => x.Name) : query.OrderByDescending(x => x.Name),
                    "departmentCode" => sortDirection == "asc" ? query.OrderBy(x => x.DepartmentCode) : query.OrderByDescending(x => x.DepartmentCode),
                    _ => query.OrderBy(x => x.Name)
                };

                // Paging
                var pagedData = await query.Skip(start).Take(length).ToListAsync();

                // Mapping (if using DTOs)
                var data = pagedData.Select(x => new DepartmentsDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    DepartmentCode = x.DepartmentCode,
                    ManagerTitle = x.ManagerTitle,
                    ParentId = x.ParentId,
                    ParentOrder = x.ParentOrder,
                    GeneralOrder = x.GeneralOrder,
                    DepTypeId = x.DepTypeId
                }).ToList();

                return (data, totalCount);
            }
            catch (Exception ex)
            {

                throw;
            }
            
        }

        public Task<IEnumerable<DepartmentsDto>> GetUserDepartmentAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<DepartmentsDto> UpdateAsync(int Id, DepartmentsDto editDto)
        {
            var existingRecord = await _departmentRepository.GetByIdAsync(Id);
            if (existingRecord == null)
                throw new Exception("Department not found");

            _mapper.Map(editDto, existingRecord);
            await _departmentRepository.UpdateAsync(existingRecord);

            return _mapper.Map<DepartmentsDto>(existingRecord);
        }

        public async Task<bool> UpdateManager(UpdateDepartmentManagerDto dto)
        {
            var department = await _departmentRepository.GetByIdAsync(dto.DepartmentId);
            department.ManagerId = dto.UserId;
            await _departmentRepository.UpdateAsync(department);
            return true;
        }
        public async Task<IEnumerable<DepartmentTypeDto>> DepartmentTypes()
        {
            try
            {
                var departmentTypes = await _departmentTypeRepository.GetAllAsync();
                return _mapper.Map<IEnumerable<DepartmentTypeDto>>(departmentTypes);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
