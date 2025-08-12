using Application.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PMSWebApp.Models;

namespace PMSWebApp.Controllers
{
    [Authorize]
    public class RolePermissionsController: Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public RolePermissionsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index()
        {
            var vm = new RolePermissionViewModel();
            vm.RoleId = "217d6db5-50a8-45d3-ae99-c5fbe016ed3f"; // Example role ID, replace with actual logic to get the role ID
            vm.RoleName="Admin"; // Example role name, replace with actual logic to get the role name
            var permissionModules = await _unitOfWork.PermissionService.GetAllAsync("217d6db5-50a8-45d3-ae99-c5fbe016ed3f");
            vm.PermissionModules = permissionModules.ToList();
            return View(vm);
        }

        public async Task<IActionResult> GetPermissions(string roleId,bool isview=false)
        {
            var vm = new RolePermissionViewModel();
            vm.IsView = isview;
            vm.RoleId = roleId;
            vm.RoleName = _unitOfWork.RoleService.GetByIdAsync(roleId).Result.Name;
            var permissionModules = await _unitOfWork.PermissionService.GetAllAsync(roleId);
            vm.PermissionModules = permissionModules.ToList();
            return PartialView("_rolePermissions", vm);
        }
       
        [HttpPost]
        public async Task<IActionResult> GetPermissions([FromBody] RolePermissionViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _unitOfWork.PermissionService.SaveRolePermissionsAsync(model.RoleId, model.SelectedPermissionIds);
                    await _unitOfWork.CompleteAsync();

                    return Json(new { success = true, message = "Permissions saved successfully." });
                }

                return Json(new { success = false, message = "Validation failed.", errors = ModelState });
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = "Validation failed.", errors = ModelState });
            }
           
        }

    }
}
