using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SponsorshipRequestApprovalProject.Domain.Entities;

namespace SponsorshipRequestApprovalProject.Infrastructure.Persistence;

public static class SponsorshipTypeSeeder
{
    public static async Task SeedSponsorshipTypesAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var types = new[]
        {
            "Event Sponsorship",
            "Community Sponsorship",
            "Educational Sponsorship",
            "Sports Sponsorship",
            "Corporate Partnership"
        };

        var utcNow = DateTimeOffset.UtcNow;

        foreach (var typeName in types)
        {
            var existing = await dbContext.SponsorshipTypes
                .FirstOrDefaultAsync(type => type.Name == typeName);

            if (existing is null)
            {
                dbContext.SponsorshipTypes.Add(new SponsorshipType
                {
                    Id = Guid.NewGuid(),
                    Name = typeName,
                    Description = null,
                    IsActive = true,
                    CreatedAt = utcNow,
                    UpdatedAt = null
                });
            }
            else if (!existing.IsActive)
            {
                existing.IsActive = true;
                existing.UpdatedAt = utcNow;
            }
        }

        await dbContext.SaveChangesAsync();
    }
}
