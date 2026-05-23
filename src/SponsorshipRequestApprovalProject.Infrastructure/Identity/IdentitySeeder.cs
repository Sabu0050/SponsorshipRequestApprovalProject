using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using SponsorshipRequestApprovalProject.Application.Common.Identity;

namespace SponsorshipRequestApprovalProject.Infrastructure.Identity;

public static class IdentitySeeder
{
    private const string DemoPassword = "Demo@12345";

    private static readonly DemoUserSeed[] DemoUsers =
    [
        new(
            Email: "requestor.demo@company.com",
            FirstName: "Requestor",
            LastName: "Demo",
            Department: "Marketing",
            Role: ApplicationRoles.Requestor),
        new(
            Email: "manager.demo@company.com",
            FirstName: "Manager",
            LastName: "Demo",
            Department: "Management",
            Role: ApplicationRoles.Manager),
        new(
            Email: "finance.demo@company.com",
            FirstName: "Finance",
            LastName: "Demo",
            Department: "Finance",
            Role: ApplicationRoles.FinanceAdmin),
        new(
            Email: "admin.demo@company.com",
            FirstName: "System",
            LastName: "Admin",
            Department: "IT",
            Role: ApplicationRoles.SystemAdmin)
    ];

    public static async Task SeedIdentityAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        foreach (var role in ApplicationRoles.All)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        foreach (var demoUser in DemoUsers)
        {
            await EnsureDemoUserAsync(userManager, demoUser);
        }
    }

    private static async Task EnsureDemoUserAsync(
        UserManager<ApplicationUser> userManager,
        DemoUserSeed demoUser)
    {
        var user = await userManager.FindByEmailAsync(demoUser.Email);
        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = demoUser.Email,
                Email = demoUser.Email,
                EmailConfirmed = true,
                FirstName = demoUser.FirstName,
                LastName = demoUser.LastName,
                Department = demoUser.Department,
                IsActive = true
            };

            var createResult = await userManager.CreateAsync(user, DemoPassword);
            if (!createResult.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Unable to create demo user '{demoUser.Email}'.");
            }
        }
        else
        {
            user.FirstName = demoUser.FirstName;
            user.LastName = demoUser.LastName;
            user.Department = demoUser.Department;
            user.IsActive = true;
            user.EmailConfirmed = true;

            var updateResult = await userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Unable to update demo user '{demoUser.Email}'.");
            }

            var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
            var resetResult = await userManager.ResetPasswordAsync(user, resetToken, DemoPassword);
            if (!resetResult.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Unable to reset password for demo user '{demoUser.Email}'.");
            }
        }

        var existingRoles = await userManager.GetRolesAsync(user);
        if (existingRoles.Count > 0)
        {
            var removeResult = await userManager.RemoveFromRolesAsync(user, existingRoles);
            if (!removeResult.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Unable to clear roles for demo user '{demoUser.Email}'.");
            }
        }

        var addResult = await userManager.AddToRoleAsync(user, demoUser.Role);
        if (!addResult.Succeeded)
        {
            throw new InvalidOperationException(
                $"Unable to assign role '{demoUser.Role}' to demo user '{demoUser.Email}'.");
        }
    }
    private readonly record struct DemoUserSeed(
        string Email,
        string FirstName,
        string LastName,
        string Department,
        string Role);
}
