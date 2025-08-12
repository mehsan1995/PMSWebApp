using Application.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PMSWebApp.Models;

namespace PMSWebApp.Controllers
{
    [Authorize]
    public class TenantsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public TenantsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> GetPagginatedData([FromForm] DataTableRequest request)
        {
            int pageSize = request.Length;
            int skip = request.Start;
            string sortColumn = request.Columns[request.Order[0].Column].Data;
            string sortDirection = request.Order[0].Dir;
            string searchValue = request.Search?.Value;

            var (data, totalCount) = await _unitOfWork.TenantService.GetPaginatedAsync(
                skip, pageSize, searchValue, sortColumn, sortDirection);

            return Json(new
            {
                draw = request.Draw,
                recordsTotal = totalCount,
                recordsFiltered = totalCount,
                data = data
            });
        }
        public async Task<IActionResult> AddEdit(int? id)
        {
            TenantViewModel vm = new TenantViewModel();
            if(id == null)
            {
                vm.Tenant = new Application.DTOs.TenantsDto();
                return View(vm);
            }
            vm.Tenant=await _unitOfWork.TenantService.GetByIdAsync(id.Value);
            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> AddEdit(TenantViewModel vm)
        {
            if(ModelState.IsValid)
            {
                if (vm.Tenant.Id > 0)
                {
                    await _unitOfWork.TenantService.UpdateAsync((int)vm.Tenant.Id, vm.Tenant);
                }
                else
                {
                    await _unitOfWork.TenantService.CreateAsync(vm.Tenant);
                   
                }
                await _unitOfWork.CompleteAsync();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(int Id)
        {
            if (Id > 0)
            {
                var vm= new TenantViewModel();
                vm.Tenant=await _unitOfWork.TenantService.GetByIdAsync(Id);
                return PartialView("_Details", vm);
            }

            return NotFound();

        }
        public async Task<IActionResult> Delete(int Id)
        {
            try
            {
                if (Id > 0)
                {
                    await _unitOfWork.TenantService.DeleteAsync(Id);
                    await _unitOfWork.CompleteAsync();
                    return Json(new { success = true, message = $"Tenant has been deleted successfully." });
                }
                return Json(new { success = false, message = $"Tenant not found." });

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred.", detail = ex.Message });
            }
           
            
        }
    }
}
