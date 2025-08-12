using Application.DTOs;
using Application.UnitOfWork;
using DAL.TenantProvider;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PMSWebApp.Models;
using System.Globalization;

namespace PMSWebApp.Controllers
{
    [Authorize]
    public class SettingsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SettingsController(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }
        public IActionResult Settings()
        {
            return View();
        }
        public async Task<IActionResult> SystemsSetting()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user != null && user.Identity != null && user.Identity.IsAuthenticated)
            {
                var userId = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                SettingsViewModel vm = new SettingsViewModel();
                var Settings = await _unitOfWork.SettingsService.GetByIdAsync(userId);
                vm.Settings = Settings == null ? new SettingsDto() : Settings;
                vm.Settings.UserId = userId;
                return View(vm);

            }

            return NotFound();


           
        }
        [HttpPost]
        public async Task<IActionResult> SystemsSetting(SettingsViewModel vm)
        {
            if (vm.Settings.Id == 0)
            {
                vm.Settings = await _unitOfWork.SettingsService.CreateAsync(vm.Settings);
            }
            else
            {
                vm.Settings = await _unitOfWork.SettingsService.UpdateAsync(vm.Settings.Id,vm.Settings);
            }
            await _unitOfWork.CompleteAsync();
            return RedirectToAction(nameof(SystemsSetting));
        }
    }
}
