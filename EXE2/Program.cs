using Microsoft.EntityFrameworkCore;
using EXE2.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;

namespace EXE2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddRazorPages();
            builder.Services.AddDbContext<Exe2Context>(options =>
  options.UseSqlServer(builder.Configuration.GetConnectionString("MyCnn")));
            builder.Services.AddSession();



            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            })
.AddCookie(options =>
{
    options.LoginPath = "/Login";
    options.LogoutPath = "/Login?handler=Logout";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
})
.AddGoogle(options =>
{
    options.ClientId = "1058708030793-huo1qo9uvvun7g239uonpge7utn8ubav.apps.googleusercontent.com";
    options.ClientSecret = "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";
    options.Scope.Add("email");
    options.SaveTokens = true;

    options.ClaimActions.MapJsonKey("email", "email");
});




            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}
