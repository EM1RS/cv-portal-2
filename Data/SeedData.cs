using CvAPI2.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

        string[] roles = { "Admin", "User" };

        // 1. Opprett roller hvis de ikke finnes
        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var result = await roleManager.CreateAsync(new Role { Name = roleName });
                Console.WriteLine($"Rolle opprettet: {roleName} (Success: {result.Succeeded})");
            }
        }

        // 2. Hent rolleobjektene
        var adminRole = await roleManager.FindByNameAsync("Admin");
        var userRole = await roleManager.FindByNameAsync("User");

        // 3. Opprett admin-bruker
        var adminEmail = "eso@trimit.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new User
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "Admin Bruker",
                EmailConfirmed = true,
            };

            var result = await userManager.CreateAsync(adminUser, "AdminPass2025!");
            Console.WriteLine($"👤 Admin opprettet: {result.Succeeded}");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
                Console.WriteLine($"Admin tildelt rolle: Admin");
            }
        }

        // Skriv ut roller admin faktisk har
        var adminRoles = await userManager.GetRolesAsync(adminUser);
        Console.WriteLine($"Roller til admin: {string.Join(", ", adminRoles)}");

        // 4. Opprett vanlig bruker
        var userEmail = "user@example.com";
        var normalUser = await userManager.FindByEmailAsync(userEmail);
        if (normalUser == null)
        {
            normalUser = new User
            {
                UserName = userEmail,
                Email = userEmail,
                FullName = "Vanlig Bruker",
                EmailConfirmed = true,
            };

            var result = await userManager.CreateAsync(normalUser, "User123!");
            Console.WriteLine($"Vanlig bruker opprettet: {result.Succeeded}");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(normalUser, "User");
                Console.WriteLine($"Bruker tildelt rolle: User");
            }
        }
    }
}
