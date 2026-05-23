using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SponsorshipRequestApprovalProject.Domain.Entities;

namespace SponsorshipRequestApprovalProject.Infrastructure.Persistence.Configurations;

public class RequestAttachmentConfiguration : IEntityTypeConfiguration<RequestAttachment>
{
    public void Configure(EntityTypeBuilder<RequestAttachment> builder)
    {
        builder.ToTable("RequestAttachments");

        builder.HasKey(attachment => attachment.Id);

        builder.Property(attachment => attachment.FileName)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(attachment => attachment.ContentType)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(attachment => attachment.FileSizeBytes)
            .IsRequired();

        builder.Property(attachment => attachment.StoragePath)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(attachment => attachment.UploadedById)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(attachment => attachment.UploadedByName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(attachment => attachment.UploadedAt)
            .IsRequired();

        builder.HasOne(attachment => attachment.SponsorshipRequest)
            .WithMany(request => request.Attachments)
            .HasForeignKey(attachment => attachment.SponsorshipRequestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(attachment => attachment.SponsorshipRequestId);
        builder.HasIndex(attachment => attachment.UploadedAt);
        builder.HasIndex(attachment => attachment.UploadedById);
    }
}
