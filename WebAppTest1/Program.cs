using AntiqueBookstore.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebAppTest1.Data;

namespace WebAppTest1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();



            //builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
            //    .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>  
                            {
                                options.SignIn.RequireConfirmedAccount = false;
                                options.Password.RequireDigit = false;
                                options.Password.RequiredLength = 4;
                                options.Password.RequireNonAlphanumeric = false;
                                options.Password.RequireUppercase = false;
                                options.Password.RequireLowercase = false;
                            })
                            .AddEntityFrameworkStores<ApplicationDbContext>()
                            .AddDefaultTokenProviders();
                            //.AddDefaultUI(); 


            // REPRODUCE ISSUE
            // 1. Program.cs - uncomment .AddIdentity 
            // 2. ApplicationDbContext.cs - inherit from IdentityDbContext<ApplicationUser>
            // 3. _LoginPartial.cshtml - switch Managers to ApplicationUser

            // ISSUE routing broken
            // https://localhost:7134/?area=Identity&page=%2FAccount%2FLogin
            // https://localhost:7134//Identity/Account/Login


            builder.Services.AddRazorPages();

            builder.Services.AddControllersWithViews();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }


            app.UseHttpsRedirection();
            app.UseStaticFiles();


            app.UseRouting();


            app.UseAuthentication();
            app.UseAuthorization();      


            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapRazorPages();


            // Add-Migration InitialAppSchema
            // Update-Database
            // Remove-Migration

            app.Run();
        }
    }
}
