using EXE2.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using System.Text;

namespace EXE2.Pages
{
   
    public class LoginModel : PageModel
    {
        private readonly Exe2Context _exe2;

        [BindProperty]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        [BindProperty]
        public string FullName { get; set; }

        public LoginModel(Exe2Context exe2)
        {
            _exe2 = exe2;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = _exe2.Users.FirstOrDefault(u => u.Email == Email && u.PasswordHash == Password);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return Page();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim("UserId", user.UserId.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(30) 
                });

      
            return user.Role switch
            {
                1 => RedirectToPage("/Index"),
                2 => RedirectToPage("/Index2"),
                _ => RedirectToPage("/AccessDenied")
            };
        }

        public IActionResult OnPostRegister()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Kiểm tra email đã tồn tại
            if (_exe2.Users.Any(u => u.Email == Email))
            {
                ModelState.AddModelError(string.Empty, "Email đã được đăng ký.");
                return Page();
            }

            // Tạo người dùng mới
            var user = new User
            {
                FullName = FullName,
                Email = Email,
                PasswordHash =Password,
                Role = 2, 
                CreatedAt = DateTime.Now
            };

            _exe2.Users.Add(user);
            _exe2.SaveChanges();

          
            return RedirectToPage("/Login");
        }
        private string HashPassword(string password)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
        public async Task<IActionResult> OnPostLogoutAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToPage("/Login");
        }



    }
}
