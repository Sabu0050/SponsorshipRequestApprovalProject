using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SponsorshipRequestApprovalProject.Domain.Entities;

namespace SponsorshipRequestApprovalProject.Infrastructure.Persistence.Configurations;

public class WorkflowHistoryConfiguration : IEntityTypeConfiguration<WorkflowHistory>
{
    public void Configure(EntityTypeBuilder<WorkflowHistory> builder)
    {
        builder.ToTable("WorkflowHistories");

        builder.HasKey(history => history.Id);

        builder.Property(history => history.Action)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(history => history.FromStatus)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(history => history.ToStatus)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(history => history.PerformedById)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(history => history.PerformedByName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(history => history.AssignedToId)
            .HasMaxLength(450);

        builder.Property(history => history.AssignedToName)
            .HasMaxLength(200);

        builder.Property(history => history.Comments)
            .HasMaxLength(2000);

        builder.Property(history => history.PerformedAt)
            .IsRequired();

        builder.HasOne(history => history.SponsorshipRequest)
            .WithMany(request => request.WorkflowHistories)
            .HasForeignKey(history => history.SponsorshipRequestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(history => history.SponsorshipRequestId);
        builder.HasIndex(history => history.Action);
        builder.HasIndex(history => history.PerformedById);
        builder.HasIndex(history => history.AssignedToId);
        builder.HasIndex(history => history.PerformedAt);
    }
}
