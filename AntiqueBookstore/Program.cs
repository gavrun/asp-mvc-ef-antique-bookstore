using AntiqueBookstore.Data;
using AntiqueBookstore.Data.Interceptors;
using AntiqueBookstore.Data.Seed;
using AntiqueBookstore.Domain.Entities;
using AntiqueBookstore.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AntiqueBookstore
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // DI container configuration and Services
            var builder = WebApplication.CreateBuilder(args);

            // Logging configuration
            // builder.Logging.ClearProviders(); // remove default logging provider
            // builder.Logging.AddConsole();
            // builder.Logging.AddDebug(); // AddFile, AddSerilog
            // builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Debug); // Information, Warning, Error, Critical

            // Access HttpContext outside of Controllers/Middleware
            // registered by default AddControllersWithViews() or AddRazorPages()
            // builder.Services.AddHttpContextAccessor();

            // Interceptor configuration
            builder.Services.AddScoped<SalesAuditInterceptor>();

            // Connection configuration
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            // Context configuration, ApplicationDbContext, SQL Server, mode Scoped
            //builder.Services.AddDbContext<ApplicationDbContext>(options =>
            //    options.UseSqlServer(connectionString));

            // Context configuration, ApplicationDbContext using IServiceProvider
            builder.Services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
            {
                // Configure the database provider
                options.UseSqlServer(connectionString);
                // options.EnableSensitiveDataLogging(); // WARNING: sensitive data in logs

                // Configure the resolved interceptor instance
                var interceptor = serviceProvider.GetRequiredService<SalesAuditInterceptor>();
                options.AddInterceptors(interceptor);
            });

            // Configure detailed error page for database-related exceptions
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();


            // Identity configuration, custom ApplicationUser, IdentityRole

            //builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //    .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                            {
                                // Conifgure Identity options
                                options.SignIn.RequireConfirmedAccount = false;
                                options.Password.RequireDigit = false;
                                options.Password.RequiredLength = 4;
                                options.Password.RequireNonAlphanumeric = false;
                                options.Password.RequireUppercase = false;
                                options.Password.RequireLowercase = false;
                            })
                            .AddEntityFrameworkStores<ApplicationDbContext>()
                            .AddDefaultTokenProviders()
                            .AddDefaultUI(); // BUG: ref. ApplicationPartManager, IdentityOptions probably overriden here


            // MVC configuration
            builder.Services.AddRazorPages(); 
            builder.Services.AddControllersWithViews();

            // Services configuration
            builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();


            // Build the application instance
            var app = builder.Build();


            // BUG: Seed user to Identity
            if (app.Environment.IsDevelopment())
            {
                await IdentitySeeder.SeedUserAsync(app);
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

            // HTTP > HTTPS
            app.UseHttpsRedirection();
            // Static files, CSS, JS, images
            app.UseStaticFiles();

            app.UseRouting();

            // Configure checks if the user logged in and has permission to access a resource
            app.UseAuthentication();
            app.UseAuthorization();

            
            // Endpoints

            // debug
            //app.MapGet("/ping", () => "pong");

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapRazorPages(); 
            //    endpoints.MapControllers();
            //});

            app.MapRazorPages(); 

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");


            // Launch the application on Kestrel
            app.Run();
        }
    }
}
