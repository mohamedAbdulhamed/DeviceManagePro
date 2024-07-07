using Microsoft.AspNetCore.Identity;
using DevicesApp.Models;
using DeviceApp.Data;

namespace DevicesApp.Data;

public static class SeedData
{
    public static async Task SeedDbDefaultData(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, AppDbContext context)
    {
        // Define roles
        var roles = new[] { "Admin", "DataEntryUser", "DeviceControllerUser" };

        // Ensure roles are created
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Get admin user details from appsettings.json
        var adminUsername = configuration["AdminUser:UserName"] ?? "Unknown";
        var adminFirstname = configuration["AdminUser:FirstName"] ?? "Unknown";
        var adminLastname = configuration["AdminUser:LastName"] ?? "Unknown";
        var adminEmail = configuration["AdminUser:Email"] ?? "Unknown";
        var adminPassword = configuration["AdminUser:Password"] ?? "Unknown";

        // Create admin user if not exists
        if (await userManager.FindByNameAsync(adminUsername) == null)
        {
            var adminUser = new ApplicationUser
            {
                UserId = Guid.NewGuid(),
                UserName = adminUsername,
                FirstName = adminFirstname,
                LastName = adminLastname,
                Email = adminEmail,
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
            else
            {
                // Log errors if any
                var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("SeedData");
                logger.LogError(string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        // Add default device types if not exists
        if (!context.DeviceTypes.Any())
        {
            var defaultDeviceTypes = new[]
            {
                new DeviceType { Name = "Water Counter" },
                new DeviceType { Name = "Power Counter" },
                new DeviceType { Name = "Gas Counter" }
            };

            await context.DeviceTypes.AddRangeAsync(defaultDeviceTypes);
            await context.SaveChangesAsync();
        }
    }
}
