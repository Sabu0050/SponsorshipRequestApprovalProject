using Microsoft.EntityFrameworkCore;
using SponsorshipRequestApprovalProject.Application.Common.Interfaces;
using SponsorshipRequestApprovalProject.Domain.Entities;

namespace SponsorshipRequestApprovalProject.UnitTests.TestDoubles;

internal class InMemoryApplicationDbContext : DbContext, IApplicationDbContext
{
    public InMemoryApplicationDbContext(DbContextOptions<InMemoryApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<SponsorshipRequest> SponsorshipRequests => Set<SponsorshipRequest>();

    public DbSet<SponsorshipType> SponsorshipTypes => Set<SponsorshipType>();

    public DbSet<WorkflowHistory> WorkflowHistories => Set<WorkflowHistory>();

    public DbSet<RequestAttachment> RequestAttachments => Set<RequestAttachment>();
}
