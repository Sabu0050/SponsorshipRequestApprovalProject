using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SponsorshipRequestApprovalProject.Infrastructure.Identity;

namespace SponsorshipRequestApprovalProject.Infrastructure.Persistence.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("AspNetUsers");

        builder.Property(user => user.FirstName)
            .HasMaxLength(100);

        builder.Property(user => user.LastName)
            .HasMaxLength(100);

        builder.Property(user => user.Department)
            .HasMaxLength(150);

        builder.Property(user => user.IsActive)
            .HasDefaultValue(true);

        builder.HasIndex(user => user.Email);
        builder.HasIndex(user => user.IsActive);
    }
}
