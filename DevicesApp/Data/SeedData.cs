using Microsoft.AspNetCore.Identity;
using DevicesApp.Models;
using DeviceApp.Data;
using Microsoft.EntityFrameworkCore;

namespace DevicesApp.Data
{
    public static class SeedData
    {
        public static async Task SeedDbDefaultData(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, AppDbContext context)
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
            var adminUsername = configuration["AdminUser:UserName"];
            var adminEmail = configuration["AdminUser:Email"];
            var adminPassword = configuration["AdminUser:Password"];

            // Create admin user if not exists
            if (await userManager.FindByNameAsync(adminUsername) == null)
            {
                var adminUser = new IdentityUser { UserName = adminUsername, Email = adminEmail, EmailConfirmed = true };
                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // Add default device types
            using (context)
            {
                if (!context.DeviceTypes.Any())
                {
                    var defaultDeviceTypes = new[]
                    {
                        new DeviceType { Name = "Water Counter" },
                        new DeviceType { Name = "Power Counter" },
                        new DeviceType { Name = "Gas Counter" }
                    };

                    context.DeviceTypes.AddRange(defaultDeviceTypes);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
