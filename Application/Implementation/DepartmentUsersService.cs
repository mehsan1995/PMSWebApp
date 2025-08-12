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
    public class DepartmentUsersService : IDepartmentUsersService
    {
        private readonly IGenericRepository<DepartmentUsers> _depatrmentUsersRepository;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _dbContext;
        public DepartmentUsersService(
            IMapper mapper,
            ApplicationDbContext applicationDbContext)
        {
            _depatrmentUsersRepository = new GenericRepository<DepartmentUsers>(applicationDbContext);
            _mapper = mapper;
            _dbContext = applicationDbContext;
        }
        public async Task<DepartmentUsersDto> CreateAsync(DepartmentUsersDto createDto)
        {
            try
            {
                var department = _mapper.Map<DepartmentUsers>(createDto);
                return _mapper.Map<DepartmentUsersDto>(await _depatrmentUsersRepository.AddAsync(department));

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
                var user = await _depatrmentUsersRepository.GetByIdAsync(Id);
                _depatrmentUsersRepository.Delete(user);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Task<IEnumerable<DepartmentUsersDto>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<DepartmentUsersListDto>> GetUserWithDepartmentAsync(int departmentId)
        {
            try
            {
                int depId = departmentId;

                var query =
                    from du in _dbContext.DepartmentUsers
                    join u in _dbContext.Users on du.UserId equals u.Id
                    join d in _dbContext.Departments on du.DepartmentId equals d.Id
                    where du.DepartmentId == depId
                    select new DepartmentUsersListDto
                    {
                        Id=du.Id,
                        UserId = u.Id,
                        DepartmentId = d.Id,
                        UserName = u.FirstName + " " + u.LastName,
                        IsManager = u.Id== d.ManagerId? true : false,
                        Managertitle = d.ManagerTitle
                    };

                return await query.ToListAsync(); // Executes the query and returns the data
            }
            catch (Exception)
            {
                throw;
            }
        }

        
    }
}
