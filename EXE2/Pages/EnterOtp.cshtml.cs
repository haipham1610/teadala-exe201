using EXE2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EXE2.Pages
{
    public class EnterOtpModel : PageModel
    {
        private readonly Exe2Context _context;

        public EnterOtpModel(Exe2Context context)
        {
            _context = context;
        }

        [BindProperty]
        public string OTP { get; set; }

        [BindProperty]
        public string NewPassword { get; set; }

        public IActionResult OnGet() => Page();

        public IActionResult OnPost()
        {
            var sessionOtp = HttpContext.Session.GetString("OTP");
            var expireTime = HttpContext.Session.GetString("OTP_Expire");
            var email = HttpContext.Session.GetString("ResetEmail");

            if (sessionOtp == null || expireTime == null || email == null)
            {
              
                TempData["ErrorMessage"] = "OTP is invalid.";
                return Page();
            }

            if (DateTime.UtcNow > DateTime.Parse(expireTime))
            {
                TempData["ErrorMessage"] = "Token đã hết hạn.";
                return Page();
            }

            if (OTP != sessionOtp)
            {
                TempData["ErrorMessage"] = "Incorrect OTP code";
                return Page();
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User does not exist.";
                return Page();
            }

            user.PasswordHash = NewPassword; 
            _context.SaveChanges();

           
            HttpContext.Session.Remove("OTP");
            HttpContext.Session.Remove("ResetEmail");
            HttpContext.Session.Remove("OTP_Expire");

            TempData["Message"] = "Password reset successful.";
            return RedirectToPage("/Login");
        }
    }
}
