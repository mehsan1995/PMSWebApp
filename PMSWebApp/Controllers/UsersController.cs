using Application.UnitOfWork;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PMSWebApp.Helper;
using PMSWebApp.Models;

namespace PMSWebApp.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public UsersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        public async Task<IActionResult> Users()
        {

            var users = await _unitOfWork.UserService.GetAllUsersAsync();
            var vm = new UsersViewModel
            {
                Users = users.ToList()
            };
            return View(vm);
        }
        public async Task<IActionResult> GetAllUsers(string searchTerm)
        {
            var users = await _unitOfWork.UserService.GetAllUsersAsync(searchTerm);
            return Json(users.Select(u => new
            {
                id = u.Id,
                text = $"{u.FirstName} {u.LastName}"
            }));

        }
        public async Task<IActionResult> GetPagginatedData([FromForm] DataTableRequest request)
        {
            int pageSize = request.Length;
            int skip = request.Start;

            string sortColumn = null;
            string sortDirection = null;

            if (request.Order?.Any() == true && request.Columns?.Count > 0)
            {
                var orderColumn = request.Order.FirstOrDefault();
                if (orderColumn != null && orderColumn.Column < request.Columns.Count)
                {
                    sortColumn = request.Columns[orderColumn.Column]?.Data;
                    sortDirection = orderColumn.Dir;
                }
            }
            string searchValue = request.Search?.Value;

            var (data, totalCount) = await _unitOfWork.UserService.GetPaginatedRolesAsync(
                skip, pageSize, searchValue, sortColumn, sortDirection);

            return Json(new
            {
                draw = request.Draw,
                recordsTotal = totalCount,
                recordsFiltered = totalCount,
                data = data
            });
        }
        public async Task<IActionResult> AddEdit(string id)
        {
            var vm = new UsersViewModel();

            var roles = await _unitOfWork.RoleService.GetAllAsync();
            vm.Roles = roles.Select(r => new SelectListItem
            {
                Value = r.Id,
                Text = r.Name
            }).ToList();

            var departments = await _unitOfWork.DepartmentService.GetAllAsync();
            vm.Departments=departments.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.Name
            }).ToList();

            if (id is not null)
            {
                vm.User = await _unitOfWork.UserService.GetUserByIdAsync(id);
                var userRoles = await _unitOfWork.RoleService.GetUserWithRolesAsync(vm.User.Id);
                var role = roles.FirstOrDefault(r => userRoles.Any(ur => ur.Name == r.Name));
                vm.RoleId = role?.Id;
            }
            else
            {
                vm.User = new Application.DTOs.UserDto();
            }

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> AddEdit(UsersViewModel usersViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (string.IsNullOrEmpty(usersViewModel.User.Id))
                    {
                        // New user
                        await _unitOfWork.UserService.CreateUserAsync(usersViewModel.User);
                    }
                    else
                    {
                        
                        await _unitOfWork.UserService.UpdateUserAsync(usersViewModel.User.Id,usersViewModel.User);
                    }

                    await _unitOfWork.CompleteAsync();
                    return RedirectToAction("Users");
                }


                var roles = await _unitOfWork.RoleService.GetAllAsync();
                usersViewModel.Roles = roles.Select(r => new SelectListItem
                {
                    Value = r.Id,
                    Text = r.Name
                }).ToList();

                var departments = await _unitOfWork.DepartmentService.GetAllAsync();
                usersViewModel.Departments = departments.Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Name
                }).ToList();
                return View(usersViewModel);
            }
            catch (Exception ex)
            {
                // Optional: log exception
                throw;
            }
        }


        public async Task<IActionResult> Details(string id)
        {
            try
            {
                var vm = new UsersViewModel();
                if (id is not null)
                {
                    var user = await _unitOfWork.UserService.GetUserByIdAsync(id);
                    if (user == null)
                    {
                        return NotFound();
                    }
                    vm.User = user;
                    return View(vm);
                }
                return RedirectToAction("Users");
            }
            catch (Exception ex)
            {
                // Handle the exception (log it, show an error message, etc.)
                return View("Error", new ErrorViewModel { RequestId = ex.Message });
            }
        }
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                if (id is not null)
                {
                    await _unitOfWork.UserService.DeleteUserAsync(id);
                    await _unitOfWork.CompleteAsync();
                }
                return RedirectToAction("Users");
            }
            catch (Exception ex)
            {
                // Handle the exception (log it, show an error message, etc.)
                return View("Error", new ErrorViewModel { RequestId = ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> ToggleStatus(string id, bool isActive)
        {
            try
            {
                await _unitOfWork.UserService.ToggleStatusAsync(id, isActive);
                await _unitOfWork.CompleteAsync();
                return Json(new { success = true, message = $"User has been {(isActive ? "enabled" : "disabled")}." });

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred.", detail = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMultiple([FromBody] List<string> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest("No IDs received");

            foreach (var id in ids)
            {
                await _unitOfWork.UserService.DeleteUserAsync(id);
            }

            await _unitOfWork.CompleteAsync();

            return Ok(new { message = "Selected records deleted successfully." });
        }

        [HttpPost]
        public async Task<IActionResult> DisableMultiple([FromBody] List<string> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest("No IDs received");

            foreach (var id in ids)
            {
                await _unitOfWork.UserService.ToggleStatusAsync(id, false);
            }

            await _unitOfWork.CompleteAsync();

            return Ok(new { message = "Selected records disabled successfully." });
        }
        [HttpPost]
        public async Task<IActionResult> EnableMultiple([FromBody] List<string> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest("No IDs received");

            foreach (var id in ids)
            {
                await _unitOfWork.UserService.ToggleStatusAsync(id, true);
            }

            await _unitOfWork.CompleteAsync();

            return Ok(new { message = "Selected records enable successfully." });
        }
    }
}
