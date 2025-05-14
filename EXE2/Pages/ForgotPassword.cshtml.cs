using EXE2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Mail;
using System.Net;

namespace EXE2.Pages
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly Exe2Context _context;
        private readonly IConfiguration _config;

        public ForgotPasswordModel(Exe2Context context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [BindProperty]
        public string Email { get; set; }

        public IActionResult OnGet() => Page();

        public IActionResult OnPost()
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == Email);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Gmail does not exist.";
                return Page();
            }

            // Tạo mã OTP
            var otp = new Random().Next(100000, 999999).ToString();

            // Lưu OTP vào Session (hoặc DB nếu muốn)
            HttpContext.Session.SetString("ResetEmail", Email);
            HttpContext.Session.SetString("OTP", otp);
            HttpContext.Session.SetString("OTP_Expire", DateTime.UtcNow.AddMinutes(5).ToString());

            // Gửi email chứa OTP
            var fromEmail = _config["EmailSettings:FromEmail"];
            var fromPassword = _config["EmailSettings:AppPassword"];

            using var smtp = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromEmail, fromPassword),
                EnableSsl = true
            };

            var mail = new MailMessage
            {
                From = new MailAddress(fromEmail),
                Subject = "Mã xác nhận đặt lại mật khẩu",
                Body = $"Mã OTP của bạn là: {otp}",
                IsBodyHtml = false
            };
            mail.To.Add(Email);
            smtp.Send(mail);

            TempData["Message"] = "Mã OTP đã được gửi về email.";
            return RedirectToPage("/EnterOtp");
        }
    }
}
