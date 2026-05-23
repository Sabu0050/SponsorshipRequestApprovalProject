using Microsoft.EntityFrameworkCore;
using SponsorshipRequestApprovalProject.Domain.Entities;

namespace SponsorshipRequestApprovalProject.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<SponsorshipRequest> SponsorshipRequests { get; }

    DbSet<SponsorshipType> SponsorshipTypes { get; }

    DbSet<WorkflowHistory> WorkflowHistories { get; }

    DbSet<RequestAttachment> RequestAttachments { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
