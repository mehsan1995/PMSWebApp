using Application.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PMSWebApp.Models;

namespace PMSWebApp.Controllers
{
    [Authorize]
    public class RolesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public RolesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index()
        {
            var vm = new RoleViewModel();
            var roles = await _unitOfWork.RoleService.GetAllAsync();
            vm.Roles = roles.ToList();
            return View();
        }
        public async Task<IActionResult> GetAllRoles(string searchTerm)
        {
            var roles = await _unitOfWork.RoleService.GetAllAsync(searchTerm);
            return Json(roles.Select(u => new
            {
                id = u.Id,
                text = u.Name
            }));

        }
        public async Task<IActionResult> Permissions()
        { 
            var vm = new RoleViewModel();   
            return PartialView("_rolePermissions",vm);
        }

        [HttpPost]
        public async Task<IActionResult> GetPagginatedData([FromForm] DataTableRequest request)
        {
            int pageSize = request.Length;
            int skip = request.Start;
            string sortColumn = request.Columns[request.Order[0].Column].Data;
            string sortDirection = request.Order[0].Dir;
            string searchValue = request.Search?.Value;

            var (data, totalCount) = await _unitOfWork.RoleService.GetPaginatedRolesAsync(
                skip, pageSize, searchValue, sortColumn, sortDirection);

            return Json(new
            {
                draw = request.Draw,
                recordsTotal = totalCount,
                recordsFiltered = totalCount,
                data = data
            });
        }


        public async Task<IActionResult> AddEdit(string? id)
        {
            var vm = new RoleViewModel();
            if (id is not null)
                vm.Role = await _unitOfWork.RoleService.GetByIdAsync(id);
            else
                vm.Role = new Application.DTOs.RoleDto();
            return PartialView("_roleAddEdit", vm);
        }

        [HttpPost]
        public async Task<IActionResult> AddEdit(RoleViewModel roleViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (string.IsNullOrEmpty(roleViewModel.Role.Id))
                    {
                        await _unitOfWork.RoleService.CreateAsync(roleViewModel.Role);
                    }
                    else
                    {

                        await _unitOfWork.RoleService.UpdateAsync(roleViewModel.Role.Id, roleViewModel.Role);
                    }
                    await _unitOfWork.CompleteAsync();

                    return Json(new { success = true, message = string.IsNullOrEmpty(roleViewModel.Role.Id)? "Role saved successfully.": "Role updated successfully" });
                }

                // Extract model state errors
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => new { Field = x.Key, Errors = x.Value.Errors.Select(e => e.ErrorMessage) });

                return Json(new { success = false, message = "Validation failed.", errors });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred.", detail = ex.Message });
            }
        }

        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _unitOfWork.RoleService.DeleteAsync(id);
                await _unitOfWork.CompleteAsync();
                return Json(new { success = true, message = "Role Deleted Successfully" });
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
                await _unitOfWork.RoleService.DeleteAsync(id);
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
                await _unitOfWork.RoleService.ToggleStatusAsync(id,false);
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
                await _unitOfWork.RoleService.ToggleStatusAsync(id, true);
            }

            await _unitOfWork.CompleteAsync();

            return Ok(new { message = "Selected records enable successfully." });
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(string id, bool isActive)
        {
            try
            {
                await _unitOfWork.RoleService.ToggleStatusAsync(id, isActive);
                await _unitOfWork.CompleteAsync();
                return Json(new { success = true, message = $"Role has been {(isActive ? "enabled" : "disabled")}." });

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred.", detail = ex.Message });
            }
        }

    }
}
