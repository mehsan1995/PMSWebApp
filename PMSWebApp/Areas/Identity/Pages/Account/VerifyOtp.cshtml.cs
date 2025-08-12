using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace PMSWebApp.Areas.Identity.Pages.Account
{
    public class VerifyOtpModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public VerifyOtpModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            public string Email { get; set; }

            [Required]
            [StringLength(6, MinimumLength = 6)]
            public string Otp { get; set; }
        }
        public void OnGet(string email)
        {
            Input = new InputModel { Email = email };
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid request.");
                return Page();
            }

            var savedOtp = await _userManager.GetAuthenticationTokenAsync(user, "MyApp", "OTP");
            if (savedOtp != Input.Otp)
            {
                ModelState.AddModelError(string.Empty, "Invalid or expired OTP.");
                return Page();
            }

            // Generate a real reset token
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            TempData["ResetToken"] = token;
            TempData["Email"] = Input.Email;

            return RedirectToPage("./ResetPassword");
        }
    }
}
