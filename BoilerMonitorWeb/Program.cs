using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using BoilerMonitorWeb.Infrastructure.Data;
using BoilerMonitorWeb.Application.Interfaces;
using BoilerMonitorWeb.Infrastructure.Repositories;
using BoilerMonitorWeb.Application.Services;
using BoilerMonitorWeb.Infrastructure.Services;
using BoilerMonitorWeb.Hubs; // <-- ADDED: Namespace mapping for the SignalR Hub pipeline

namespace BoilerMonitorWeb
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // =========================================================================
            // 1. DATABASE & IDENTITY SERVICE REGISTRATION
            // =========================================================================
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;

                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.ExpireTimeSpan = TimeSpan.FromHours(8);
            });

            // =========================================================================
            // 2. CUSTOM INDUSTRIAL BACKEND SERVICES REGISTRATION
            // =========================================================================
            builder.Services.AddScoped<IAlarmRepository, AlarmRepository>();
            builder.Services.AddScoped<IAlarmEngine, AlarmEngine>();
            builder.Services.AddScoped<IAlarmManager, AlarmManager>();

            // --- ADDED FOR STAGE 9 COMPLIANCE RUNS ---
            builder.Services.AddScoped<IBatchValidationService, BatchValidationService>();

            // --- SIGNALR CORE ENGINE & BACKGROUND SIMULATOR PIPELINE ---
            builder.Services.AddSignalR(); // <-- ADDED
            builder.Services.AddHostedService<TelemetrySimulator>(); // <-- ADDED

            // Register custom frontend data bridge services
            builder.Services.AddScoped<BoilerDataService>();
            builder.Services.AddScoped<IBoilerService>(sp => sp.GetRequiredService<BoilerDataService>());
            builder.Services.AddScoped<ITelemetryService>(sp => sp.GetRequiredService<BoilerDataService>());
            builder.Services.AddScoped<IAlarmService>(sp => sp.GetRequiredService<BoilerDataService>());

            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // =========================================================================
            // 3. AUTOMATIC ROLE & USER SEEDING WORKFLOW
            // =========================================================================
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    await SeedRolesAndAdminAsync(services);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding identity accounts.");
                }
            }

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            // Map standard controller endpoints
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Dashboard}/{action=Index}/{id?}");

            // --- ROUTE HUB MAPPING ENGINE ---
            app.MapHub<TelemetryHub>("/telemetryHub"); // <-- ADDED

            await app.RunAsync();
        }

        private static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            string[] roleNames = { "Operator", "Engineer", "Admin" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            async Task EnsureUserCreatedAsync(string email, string password, string role)
            {
                if (await userManager.FindByEmailAsync(email) == null)
                {
                    var user = new IdentityUser
                    {
                        UserName = email,
                        Email = email,
                        EmailConfirmed = true
                    };
                    var result = await userManager.CreateAsync(user, password);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, role);
                    }
                }
            }

            await EnsureUserCreatedAsync("admin@boilerplant.com", "Admin@123", "Admin");
            await EnsureUserCreatedAsync("engineer@boilerplant.com", "Engineer@123", "Engineer");
            await EnsureUserCreatedAsync("operator@boilerplant.com", "Operator@123", "Operator");
        }
    }
}