using Application.DTOs;
using DAL.GenericRepository;
using DAL.Models;

namespace Application.Interfaces
{
    public interface IUserService
    {
        Task<(List<UserListDto> Data, int TotalCount)> GetPaginatedRolesAsync(
    int start, int length, string searchValue, string sortColumn, string sortDirection);
        Task<UserDto> CreateUserAsync(UserDto createUserDto);
        Task<UserDto> GetUserByIdAsync(string userId);
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<IEnumerable<UserListDto>> GetAllUsersAsync(string searchTerm);
        Task<UserDto> UpdateUserAsync(string userId, UserDto editUserDto);
        Task DeleteUserAsync(string userId);
        Task ToggleStatusAsync(string Id, bool status);
        //Task<ResponseDto> CreateUserAsync(UserDto createUserDto);
        //Task<ResponseDto> GetUserByIdAsync(string userId);
        //Task<ResponseDto> GetAllUsersAsync();
        //Task<ResponseDto> UpdateUserAsync(string userId, UserDto editUserDto);
        //Task<ResponseDto> DeleteUserAsync(string userId);
    }
}
