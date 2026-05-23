using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using SponsorshipRequestApprovalProject.Application.Common.Identity;

namespace SponsorshipRequestApprovalProject.Infrastructure.Identity;

public static class IdentitySeeder
{
    public static async Task SeedIdentityAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        foreach (var role in ApplicationRoles.All)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }
}
