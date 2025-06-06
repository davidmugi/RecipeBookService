using Microsoft.AspNetCore.Identity;
using RecipeBookService.Configurations.SeedData;

namespace RecipeBookService.Configurations;

public static class IdentityDataSeeder
{
    public static async Task SeedDataAsync(IServiceProvider serviceProvider)
    {
        var config = serviceProvider.GetRequiredService<IConfiguration>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

        var seedData = config.GetSection("SeedData").Get<SeedDataOptions>();

        foreach (var roleName in seedData.Roles.Distinct())
            if (!await roleManager.RoleExistsAsync(roleName))
                await roleManager.CreateAsync(new IdentityRole(roleName));

        foreach (var seedUser in seedData.Users)
        {
            var user = await userManager.FindByEmailAsync(seedUser.Email);
            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = seedUser.Email,
                    Email = seedUser.Email,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, seedUser.Password);
                if (!result.Succeeded)
                    throw new Exception(
                        $"Failed to create user {seedUser.Email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            if (!await userManager.IsInRoleAsync(user, seedUser.Role))
                await userManager.AddToRoleAsync(user, seedUser.Role);
        }
    }
}