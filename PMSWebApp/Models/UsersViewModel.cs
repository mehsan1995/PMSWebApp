using Application.DTOs;
using DAL.Models;
using System.ComponentModel.DataAnnotations;

namespace PMSWebApp.Models
{
    public class UsersViewModel:SelectItemsViewModel
    {
        public UserDto? User { get; set; }

        [Required(ErrorMessage = "Role is required")]
        [Display(Name = "Role")]
        public string? RoleId { get; set; }
        public List<UserListDto>  UsersList { get; set; } = new List<UserListDto>();
        public List<UserDto>  Users { get; set; } = new List<UserDto>();

    }
}
