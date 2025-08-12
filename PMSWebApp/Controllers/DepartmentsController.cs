using Application.DTOs;
using Application.UnitOfWork;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PMSWebApp.Models;

namespace PMSWebApp.Controllers
{

    [Authorize]
    public class DepartmentsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public DepartmentsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index()
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
            var (data, totalCount) = await _unitOfWork.DepartmentService.GetPaginatedAsync(
                skip, pageSize, searchValue, sortColumn, sortDirection);
            return Json(new
            {
                draw = request.Draw,
                recordsTotal = totalCount,
                recordsFiltered = totalCount,
                data = data
            });
        }

        public async Task<IActionResult> AddEdit(int? id, int? parentId)
        {
            CreateDepartmentViewModel vm = new CreateDepartmentViewModel();
            vm.ParentId = parentId;
            var Departments = await _unitOfWork.DepartmentService.GetAllAsync();

            if (id.HasValue)
            {
                var department = await _unitOfWork.DepartmentService.GetByIdAsync(id.Value);
                if (department == null)
                {
                    return NotFound();
                }
                vm.Department = department;

                vm.DepartmentList = Departments.Where(x => x.Id != id).Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Name
                }).ToList();
            }
            else
            {
                vm.Department = new Application.DTOs.DepartmentsDto();
                vm.Department.ParentId = parentId ?? null;
                vm.DepartmentList = Departments.Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Name
                }).ToList();
            }
            var DepartmentsType = await _unitOfWork.DepartmentService.DepartmentTypes();
            vm.DepartmentsType = DepartmentsType.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.Name
            }).ToList();




            return PartialView("_AddEdit", vm);
        }

        [HttpPost]
        public async Task<IActionResult> AddEdit(DepartmentsDto Department)
        {
            if (ModelState.IsValid)
            {
                if (Department.Id > 0)
                {
                    await _unitOfWork.DepartmentService.UpdateAsync(Department.Id, Department);
                }
                else
                {
                    await _unitOfWork.DepartmentService.CreateAsync(Department);
                }
                await _unitOfWork.CompleteAsync();

                return Json(new { success = true, message = "Department saved successfully." });
            }

            // Return validation errors as JSON
            var errors = ModelState.Where(ms => ms.Value.Errors.Any())
                                   .Select(ms => new
                                   {
                                       field = ms.Key,
                                       errors = ms.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                                   });

            return Json(new { success = false, errors });
        }

        public async Task<IActionResult> Details(int Id)
        {
            //vm.DepartmentsTree =await _unitOfWork.DepartmentService.GetDepartmentTreeAsync(Id);
            ViewBag.Id = Id;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetDepartmentTree(int Id)
        {
            try
            {
                var Departments = await _unitOfWork.DepartmentService.GetDepartmentTreeAsync(Id);
                return Json(new { success = true, data = Departments });
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = ex.Message });
            }

        }

        [HttpPost]
        public async Task<IActionResult> AssignManager([FromBody] UpdateDepartmentManagerDto dto)
        {
            try
            {
                await _unitOfWork.DepartmentService.UpdateManager(dto);
                await _unitOfWork.CompleteAsync();
                return Json(new { success = true, message = "Successfully assigned" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            
        }


    }
}
