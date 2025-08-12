using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using DAL;
using DAL.GenericRepository;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace Application.Implementation
{
    public class UserService : IUserService
    {
        private readonly IGenericRepository<ApplicationUser> _userRepository;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ApplicationDbContext _dbContext;
        public UserService(IMapper mapper,
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            ApplicationDbContext applicationDbContext
            )
        {
            _userRepository = new GenericRepository<ApplicationUser>(applicationDbContext);
            _mapper = mapper;
            _userManager = userManager;
            _emailStore = GetEmailStore();
            _userStore = userStore;
            _dbContext = applicationDbContext;
        }
        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
        public async Task<UserDto> CreateUserAsync(UserDto createUserDto)
        {
            try
            {
                var user = _mapper.Map<ApplicationUser>(createUserDto);
                user.Id = Guid.NewGuid().ToString(); 
                user.UserName = user.Email;
                user.NormalizedEmail = _userManager.NormalizeEmail(user.Email);
                user.NormalizedUserName = _userManager.NormalizeName(user.Email);
                return _mapper.Map<UserDto>(await _userRepository.AddAsync(user));

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task DeleteUserAsync(string userId)
        {
            try
            {
               var user= await _userRepository.GetByIdAsync(userId);
                _userRepository.Delete(user);
                return;
            }
            catch (Exception)
            {

                throw;
            }
          
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            try
            {
                var users = await _userRepository.GetAllAsync();
                return _mapper.Map<IEnumerable<UserDto>>(users);
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        public async Task<IEnumerable<UserListDto>> GetAllUsersAsync( string searchTerm)
        {
            var query = from user in _dbContext.Users
                        select new UserListDto
                        {
                            Id = user.Id,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            Email = user.Email
                        };

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.ToLower();
                query = query.Where(u =>
                    u.FirstName.ToLower().Contains(term) ||
                    u.LastName.ToLower().Contains(term) ||
                    u.Email.ToLower().Contains(term)) ;
            }

            return await query.ToListAsync();
        }

        public async Task<UserDto> GetUserByIdAsync(string userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            return _mapper.Map<UserDto>(user);
        }
        public async Task ToggleStatusAsync(string Id, bool status)
        {
            try
            {
                var role = await _userRepository.GetByIdAsync(Id);
                role.IsActive = status;
                await _userRepository.UpdateAsync(role);
            }
            catch (Exception)
            {

                throw;
            }

        }
        public async Task<UserDto> UpdateUserAsync(string userId, UserDto editUserDto)
        {
            var existingUser = await _userRepository.GetByIdAsync(userId);
            if (existingUser == null)
                throw new Exception("User not found");

            // Map only updated fields from DTO to the existing entity
            _mapper.Map(editUserDto, existingUser);

            existingUser.NormalizedEmail = _userManager.NormalizeEmail(existingUser.Email);
            existingUser.NormalizedUserName = _userManager.NormalizeName(existingUser.Email);

            await _userRepository.UpdateAsync(existingUser);

            return _mapper.Map<UserDto>(existingUser);
        }

        public async Task<(List<UserListDto> Data, int TotalCount)> GetPaginatedRolesAsync(int start, int length, string searchValue, string sortColumn, string sortDirection)
        {
            var query =from user in _dbContext.Users
                       join department in _dbContext.Departments on user.DepartmentsId equals department.Id into deptGroup
                       from department in deptGroup.DefaultIfEmpty()
                       join userRole in _dbContext.UserRoles on user.Id equals userRole.UserId into urGroup
                       from userRole in urGroup.DefaultIfEmpty()
                       join role in _dbContext.Roles on userRole.RoleId equals role.Id into roleGroup
                       from role in roleGroup.DefaultIfEmpty()
                       select new 
                       {
                           user.Id,
                           user.FirstName,
                            user.LastName,
                           user.Email,
                           user.PhoneNumber,
                            user.NationalID,
                           user.EmployeeID,
                           user.IsActive,
                           user.JobTitle,
                           DepartmentName = department != null ? department.Name : null,
                           RoleName = role != null ? role.Name : null
                       };

            if (!string.IsNullOrWhiteSpace(searchValue))
            {
                query =
                    from user in query
                    where user.FirstName.Contains(searchValue)
                       || user.LastName.Contains(searchValue)
                       || user.Email.Contains(searchValue)
                       || user.PhoneNumber.Contains(searchValue)
                       || user.EmployeeID.Contains(searchValue)
                       || user.NationalID.Contains(searchValue)
                       || (user.DepartmentName != null && user.DepartmentName.Contains(searchValue))
                       || (user.RoleName != null && user.RoleName.Contains(searchValue))
                    select user;
            }

            var totalCount = await query.CountAsync();

            query = sortColumn switch
            {
                "name" => sortDirection == "asc"
                    ? (from q in query orderby q.FirstName ascending select q)
                    : (from q in query orderby q.FirstName descending select q),

                "email" => sortDirection == "asc"
                ? (from q in query orderby q.Email ascending select q)
                : (from q in query orderby q.Email descending select q),
                "phoneNumber" => sortDirection == "asc"
                    ? (from q in query orderby q.PhoneNumber ascending select q)
                    : (from q in query orderby q.PhoneNumber descending select q),
                "nationalID" => sortDirection == "asc"
                ? (from q in query orderby q.NationalID ascending select q)
                : (from q in query orderby q.NationalID descending select q),
                "employeeID" => sortDirection == "asc"
                    ? (from q in query orderby q.EmployeeID ascending select q)
                    : (from q in query orderby q.EmployeeID descending select q),
                "jobTitle" => sortDirection == "asc"
                    ? (from q in query orderby q.JobTitle ascending select q)
                    : (from q in query orderby q.JobTitle descending select q),

                "role" => sortDirection == "asc"
                    ? (from q in query orderby q.RoleName ascending select q)
                    : (from q in query orderby q.RoleName descending select q),

                "department" => sortDirection == "asc"
                    ? (from q in query orderby q.DepartmentName ascending select q)
                    : (from q in query orderby q.DepartmentName descending select q),

                _ => (from q in query orderby q.FirstName ascending select q)
            };

            // Paging
            var pagedData = await(
                from user in query
                select user
            ).Skip(start).Take(length).ToListAsync();
            var data = pagedData.Select(user => new UserListDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Name = user.FirstName+" "+user.LastName,
                JobTitle = user.JobTitle,
                Email = user.Email,
                EmployeeID = user.EmployeeID,
                NationalID = user.NationalID,
                PhoneNumber = user.PhoneNumber, 
                IsActive = user.IsActive,   
                DepartmentName = user.DepartmentName,
                RoleName = user.RoleName
            }).ToList();

            return (data, totalCount);



        }





        //public async Task<ResponseDto> CreateUserAsync(UserDto createUserDto)
        //{

        //    var response = new ResponseDto();
        //    try
        //    {
        //        var user = _mapper.Map<ApplicationUser>(createUserDto);
        //        user.Id = Guid.NewGuid().ToString();
        //        //var result = await _userManager.CreateAsync(user, createUserDto.Password);
        //        var result = await _userManager.CreateAsync(user);
        //        response.Status = result.Succeeded;
        //        response.Data = _mapper.Map<UserDto>(user);
        //        response.Message = result.Succeeded? "User created successfully": "Error creating user: " + string.Join(", ", result.Errors.Select(e => e.Description));
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Status = false;
        //        response.Message="An error occurred while creating the user:"+ ex.Message;
        //    }

        //    return response;



        //}

        //public async Task<ResponseDto> GetUserByIdAsync(string userId)
        //{

        //    var response = new ResponseDto();
        //    try
        //    {
        //        var user = await _userManager.FindByIdAsync(userId);
        //        if (user is not null)
        //        {
        //            response.Data = _mapper.Map<UserDto>(user);
        //            response.Status = true;
        //            response.Message = "User retrieved successfully";
        //        }
        //        else
        //        {
        //            response.Status = false;
        //            response.Message = "User not found";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Status = false;   
        //        response.Message = "An error occurred while retrieving the user: " + ex.Message;
        //    }


        //    return response;
        //}

        //public async Task<ResponseDto> GetAllUsersAsync()
        //{
        //    var response = new ResponseDto();
        //    try
        //    {
        //        var users = await _userManager.Users.ToListAsync();
        //        if (users == null || !users.Any())
        //        {
        //            response.Status = false;
        //            response.Message = "No users found";
        //            return response;
        //        }

        //        response.Status = true;
        //        response.Data = _mapper.Map<List<UserDto>>(users);
        //        response.Message = "Users retrieved successfully";
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Status= false;
        //        response.Message = "An error occurred while retrieving users: " + ex.Message;
        //    }

        //    return response;
        //}

        //public async Task<ResponseDto> UpdateUserAsync(string userId, UserDto editUserDto)
        //{
        //    var response = new ResponseDto();
        //    try
        //    {
        //        var user = await _userManager.FindByIdAsync(userId);

        //        if (user == null) throw new Exception("User not found");
        //        _mapper.Map(editUserDto, user);

        //        var result = await _userManager.UpdateAsync(user);

        //        response.Status = result.Succeeded;
        //        response.Data = _mapper.Map<UserDto>(user);
        //        response.Message = result.Succeeded ? "User updated successfully" : "Error updating user: " + string.Join(", ", result.Errors.Select(e => e.Description));

        //    }
        //    catch (Exception ex)
        //    {
        //        response.Status = false;
        //        response.Message = "An error occurred while updating the user: " + ex.Message;
        //    }
        //    return response;
        //}

        //public async Task<ResponseDto> DeleteUserAsync(string userId)
        //{
        //    var respo = new ResponseDto();
        //    try
        //    {
        //        var user = await _userManager.FindByIdAsync(userId);
        //        if (user == null) throw new Exception("User not found");
        //        var result = await _userManager.DeleteAsync(user);

        //        respo.Status = result.Succeeded;
        //        respo.Message= result.Succeeded ? "User deleted successfully" : "Error deleting user: " + string.Join(", ", result.Errors.Select(e => e.Description));
        //    }
        //    catch (Exception ex)
        //    {
        //        respo.Status = false;
        //        respo.Message = "An error occurred while deleting the user: " + ex.Message; 
        //    }

        //    return respo;
        //}
    }
}
