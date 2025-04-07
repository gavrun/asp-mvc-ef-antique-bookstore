using AntiqueBookstore.Data;
using AntiqueBookstore.Data.Seed;
using AntiqueBookstore.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AntiqueBookstore
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // DI container configuration and services

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            // Context configuration, ApplicationDbContext, SQL Server, mode Scoped
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
                //UseNpgsql()

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            // Identity configuration
            //builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //    .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                            {
                                // Conifgure Identity options
                                //options.SignIn.RequireConfirmedAccount = true;
                                options.Password.RequireDigit = false;
                                options.Password.RequiredLength = 4;
                                options.Password.RequireNonAlphanumeric = false;
                                options.Password.RequireUppercase = false;
                                options.Password.RequireLowercase = false;
                            })
                            .AddEntityFrameworkStores<ApplicationDbContext>()
                            .AddDefaultTokenProviders();

            // MVC configuration
            builder.Services.AddRazorPages(); // INFO: load Razor first for Indentity
            builder.Services.AddControllersWithViews();
            

            // Build the application instance
            var app = builder.Build();

            // Seed non-static data 
            if (app.Environment.IsDevelopment())
            {
                await DataSeeder.SeedDatabaseAsync(app);
            }

            // Middleware conveyor pipeline

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                // debug 
                app.UseMigrationsEndPoint();
                app.UseDeveloperExceptionPage();
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

            // Endpoints
            app.MapRazorPages(); // INFO: load Razor first for Indentity

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            
            // Launch the application on Kestrel
            app.Run();
        }
    }
}
