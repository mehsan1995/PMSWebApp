using Application.DTOs;
using Application.UnitOfWork;
using DAL.TenantProvider;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PMSWebApp.Models;

namespace PMSWebApp.Controllers
{
    [Authorize]
    public class SettingsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITenantProvider _tenantProvider;

        public SettingsController(IUnitOfWork unitOfWork, ITenantProvider tenantProvider)
        {
                _unitOfWork = unitOfWork;
               _tenantProvider = tenantProvider;
        }
        public IActionResult Settings()
        {
            return View();
        }
        public async Task<IActionResult> SystemsSetting()
        {
            var tenantId= _tenantProvider.GetTenantId();

            TenantSettingsViewModel vm= new TenantSettingsViewModel();
            var TenantSettings=await _unitOfWork.SettingsService.GetByIdAsync(tenantId);
            vm.TenantSettings = TenantSettings==null? new TenantSettingsDto() : TenantSettings;
            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> SystemsSetting(TenantSettingsViewModel vm)
        {
            if (vm.TenantSettings.Id == 0)
            {
                vm.TenantSettings = await _unitOfWork.SettingsService.CreateAsync(vm.TenantSettings);
            }
            else
            {
                vm.TenantSettings = await _unitOfWork.SettingsService.UpdateAsync(vm.TenantSettings.Id,vm.TenantSettings);
            }
            await _unitOfWork.CompleteAsync();
            return RedirectToAction(nameof(SystemsSetting));
        }
    }
}
