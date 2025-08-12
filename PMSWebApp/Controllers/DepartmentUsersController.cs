using Application.DTOs;
using Application.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PMSWebApp.Models;

namespace PMSWebApp.Controllers
{
    [Authorize]
    public class DepartmentUsersController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
        public DepartmentUsersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] DepartmentUsersDto request)
        {
            try
            {

                await _unitOfWork.DepartmentUsersService.CreateAsync(request);
                await _unitOfWork.CompleteAsync();

                return Json(new { success = true,message="User added successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = true,message=ex.Message });
            }
           

          
        }
        [HttpGet]
        public async Task<IActionResult> GetUsersList(int id)
        {
            DepartmentUsersViewModel departmentUsersViewModel = new DepartmentUsersViewModel();
            var users = await _unitOfWork.DepartmentUsersService.GetUserWithDepartmentAsync(id);

            departmentUsersViewModel.DepartmentId = id;
            departmentUsersViewModel.Users = users.ToList();
            return PartialView("_DepartmentUsers", departmentUsersViewModel);
        }

        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                await _unitOfWork.DepartmentUsersService.DeleteAsync(id);
                await _unitOfWork.CompleteAsync();
                return Json(new
                {
                    success = true
                });
            }
            catch (Exception ex)
            {

                return Json(new
                {
                    success = false,
                    message=ex.Message
                });
            }
           
        }
    }
}
