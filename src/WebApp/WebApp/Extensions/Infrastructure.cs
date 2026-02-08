using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;

namespace WebApp.Extensions;

public static class InfrastructureExtensions
{
    public static async Task InitializeInfrastructureAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var connStr = app.Configuration.GetConnectionString("DefaultConnection");
        if (connStr?.StartsWith("Data Source=") == true)
        {
            var dir = Path.GetDirectoryName(connStr.Replace("Data Source=", ""));
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                try 
                {
                    Directory.CreateDirectory(dir);
                }
                catch (Exception ex)
                {
                    app.Logger.LogError(ex, "Failed to create directory {Dir}", dir);
                }
            }
        }

        db.Database.Migrate();

        var adminEmail = app.Configuration["AdminSeed:Email"]
            ?? (app.Environment.IsDevelopment() ? "admin@localhost" : null);
        var adminPassword = app.Configuration["AdminSeed:Password"]
            ?? (app.Environment.IsDevelopment() ? "Admin123!" : null);

        if (adminEmail is not null && adminPassword is not null)
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            if (await userManager.FindByEmailAsync(adminEmail) is null)
            {
                var admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(admin, adminPassword);
            }
        }
    }
}
