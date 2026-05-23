using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SponsorshipRequestApprovalProject.Domain.Entities;
using SponsorshipRequestApprovalProject.Domain.Enums;

namespace SponsorshipRequestApprovalProject.Infrastructure.Persistence.Configurations;

public class SponsorshipRequestConfiguration : IEntityTypeConfiguration<SponsorshipRequest>
{
    public void Configure(EntityTypeBuilder<SponsorshipRequest> builder)
    {
        builder.ToTable("SponsorshipRequests");

        builder.HasKey(request => request.Id);

        builder.Property(request => request.RequestNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(request => request.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(request => request.Description)
            .HasMaxLength(4000)
            .IsRequired();

        builder.Property(request => request.RequesterId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(request => request.RequesterName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(request => request.RequesterEmail)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(request => request.SponsorName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(request => request.RequestedAmount)
            .HasPrecision(18, 2);

        builder.Property(request => request.CurrencyCode)
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(request => request.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .HasDefaultValue(SponsorshipRequestStatus.Draft)
            .IsRequired();

        builder.Property(request => request.CurrentApproverId)
            .HasMaxLength(450);

        builder.Property(request => request.CurrentApproverName)
            .HasMaxLength(200);

        builder.Property(request => request.FinalDecisionById)
            .HasMaxLength(450);

        builder.Property(request => request.FinalDecisionByName)
            .HasMaxLength(200);

        builder.Property(request => request.DecisionNotes)
            .HasMaxLength(2000);

        builder.Property(request => request.CreatedAt)
            .IsRequired();

        builder.HasOne(request => request.SponsorshipType)
            .WithMany(type => type.SponsorshipRequests)
            .HasForeignKey(request => request.SponsorshipTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(request => request.RequestNumber)
            .IsUnique();

        builder.HasIndex(request => request.Status);
        builder.HasIndex(request => request.RequesterId);
        builder.HasIndex(request => request.SponsorshipTypeId);
        builder.HasIndex(request => request.CurrentApproverId);
        builder.HasIndex(request => request.CreatedAt);
    }
}
