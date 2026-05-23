using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SponsorshipRequestApprovalProject.Domain.Entities;
using SponsorshipRequestApprovalProject.Infrastructure.Identity;

namespace SponsorshipRequestApprovalProject.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<SponsorshipRequest> SponsorshipRequests => Set<SponsorshipRequest>();

    public DbSet<SponsorshipType> SponsorshipTypes => Set<SponsorshipType>();

    public DbSet<WorkflowHistory> WorkflowHistories => Set<WorkflowHistory>();

    public DbSet<RequestAttachment> RequestAttachments => Set<RequestAttachment>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditValues();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        ApplyAuditValues();
        return base.SaveChanges();
    }

    private void ApplyAuditValues()
    {
        var now = DateTimeOffset.UtcNow;

        foreach (var entry in ChangeTracker.Entries<SponsorshipRequest>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
            }
        }

        foreach (var entry in ChangeTracker.Entries<SponsorshipType>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
            }
        }

        foreach (var entry in ChangeTracker.Entries<RequestAttachment>())
        {
            if (entry.State == EntityState.Added && entry.Entity.UploadedAt == default)
            {
                entry.Entity.UploadedAt = now;
            }
        }

        foreach (var entry in ChangeTracker.Entries<WorkflowHistory>())
        {
            if (entry.State == EntityState.Added && entry.Entity.PerformedAt == default)
            {
                entry.Entity.PerformedAt = now;
            }
        }
    }
}
