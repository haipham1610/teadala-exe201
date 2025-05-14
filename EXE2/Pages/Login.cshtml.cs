using EXE2.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;

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

                TempData["ErrorMessage"] = "Invalid login attempt.";
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
                2 => RedirectToPage("/Index"),
                _ => RedirectToPage("/AccessDenied")
            };
        }

        public IActionResult OnPostRegister()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }


            if (_exe2.Users.Any(u => u.Email == Email))
            {

                TempData["ErrorMessage"] = "Email has been registered";
                return Page();
            }

            var user = new User
            {
                FullName = FullName,
                Email = Email,
                PasswordHash = Password,
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

        //public async Task<IActionResult> OnGetGoogleAsync()
        //{
        //    var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        //    if (result?.Principal != null)
        //    {
        //        // Lấy thông tin từ Google
        //        var email = result.Principal.FindFirstValue(ClaimTypes.Email);
        //        var name = result.Principal.FindFirstValue(ClaimTypes.Name);

        //        // Kiểm tra xem người dùng đã tồn tại chưa, nếu chưa thì tạo mới
        //        var user = _exe2.Users.FirstOrDefault(u => u.Email == email);
        //        if (user == null)
        //        {
        //            user = new User
        //            {
        //                FullName = name,
        //                Email = email,
        //                PasswordHash = "123",
        //                Role = 2, 
        //                CreatedAt = DateTime.Now
        //            };
        //            _exe2.Users.Add(user);
        //            _exe2.SaveChanges();
        //        }

        //        var claims = new List<Claim>
        //    {
        //        new Claim(ClaimTypes.Name, name),
        //        new Claim(ClaimTypes.Email, email),
        //        new Claim(ClaimTypes.Role, user.Role.ToString()),
        //        new Claim("UserId", user.UserId.ToString())
        //    };

        //        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        //        await HttpContext.SignInAsync(
        //            CookieAuthenticationDefaults.AuthenticationScheme,
        //            new ClaimsPrincipal(claimsIdentity),
        //            new AuthenticationProperties
        //            {
        //                IsPersistent = true,
        //                ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
        //            });

        //        return RedirectToPage("/");
        //    }

        //    // Nếu người dùng chưa đăng nhập Google, yêu cầu đăng nhập
        //    return Challenge(new AuthenticationProperties { RedirectUri = "/Login" }, GoogleDefaults.AuthenticationScheme);
        //}

        public async Task<IActionResult> OnGetGoogleAsync()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (result?.Principal != null)
            {
                // Lấy thông tin từ Google
                var email = result.Principal.FindFirstValue(ClaimTypes.Email);
                var name = result.Principal.FindFirstValue(ClaimTypes.Name);

                // Kiểm tra xem người dùng đã tồn tại chưa
                var user = await _exe2.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null)
                {
                    user = new User
                    {
                        FullName = name,
                        Email = email,
                        PasswordHash = "123", // Không mã hóa, mật khẩu mặc định
                        Role = 2, // Mặc định role là 2
                        CreatedAt = DateTime.Now
                    };

                    _exe2.Users.Add(user);
                    await _exe2.SaveChangesAsync();
                }

                // Tạo claims và đăng nhập
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, name),
            new Claim(ClaimTypes.Email, email),
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

                // Sửa đường dẫn tới trang Index
                return RedirectToPage("/Index");
            }

            // Nếu chưa đăng nhập Google, chuyển hướng đến đăng nhập
            return Challenge(new AuthenticationProperties { RedirectUri = "/Login" }, GoogleDefaults.AuthenticationScheme);
        }

    }
}
