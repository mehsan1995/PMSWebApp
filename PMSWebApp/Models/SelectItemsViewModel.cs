using Microsoft.AspNetCore.Mvc.Rendering;

namespace PMSWebApp.Models
{
    public class SelectItemsViewModel
    {
        public List<SelectListItem> Roles { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Departments { get; set; } = new List<SelectListItem>();
    }
}
