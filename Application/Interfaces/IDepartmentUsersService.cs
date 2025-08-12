using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IDepartmentUsersService
    {
        Task<DepartmentUsersDto> CreateAsync(DepartmentUsersDto createDto);
        Task<IEnumerable<DepartmentUsersListDto>> GetUserWithDepartmentAsync(int Id);
        Task<IEnumerable<DepartmentUsersDto>> GetAllAsync();
        Task DeleteAsync(int Id);
    }
}
