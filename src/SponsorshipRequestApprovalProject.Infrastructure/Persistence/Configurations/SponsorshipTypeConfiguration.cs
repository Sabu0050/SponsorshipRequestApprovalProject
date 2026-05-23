using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SponsorshipRequestApprovalProject.Domain.Entities;

namespace SponsorshipRequestApprovalProject.Infrastructure.Persistence.Configurations;

public class SponsorshipTypeConfiguration : IEntityTypeConfiguration<SponsorshipType>
{
    public void Configure(EntityTypeBuilder<SponsorshipType> builder)
    {
        builder.ToTable("SponsorshipTypes");

        builder.HasKey(type => type.Id);

        builder.Property(type => type.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(type => type.Description)
            .HasMaxLength(1000);

        builder.Property(type => type.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(type => type.CreatedAt)
            .IsRequired();

        builder.HasIndex(type => type.Name)
            .IsUnique();

        builder.HasIndex(type => type.IsActive);
    }
}
