using Microsoft.EntityFrameworkCore;
using SponsorshipRequestApprovalProject.Application.Common.CQRS;
using SponsorshipRequestApprovalProject.Application.Common.Interfaces;
using SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.DTOs;

namespace SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.Queries.GetSponsorshipRequestById;

public class GetSponsorshipRequestByIdQueryHandler(IApplicationDbContext dbContext)
    : IQueryHandler<GetSponsorshipRequestByIdQuery, SponsorshipRequestDetailDto?>
{
    public Task<SponsorshipRequestDetailDto?> Handle(
        GetSponsorshipRequestByIdQuery request,
        CancellationToken cancellationToken)
    {
        return dbContext.SponsorshipRequests
            .AsNoTracking()
            .Where(sponsorshipRequest => sponsorshipRequest.Id == request.SponsorshipRequestId)
            .Select(sponsorshipRequest => new SponsorshipRequestDetailDto(
                sponsorshipRequest.Id,
                sponsorshipRequest.RequestNumber,
                sponsorshipRequest.Title,
                sponsorshipRequest.Description,
                sponsorshipRequest.SponsorshipTypeId,
                sponsorshipRequest.SponsorshipType == null ? string.Empty : sponsorshipRequest.SponsorshipType.Name,
                sponsorshipRequest.RequesterId,
                sponsorshipRequest.RequesterName,
                sponsorshipRequest.RequesterEmail,
                sponsorshipRequest.SponsorName,
                sponsorshipRequest.RequestedAmount,
                sponsorshipRequest.CurrencyCode,
                sponsorshipRequest.EventDate,
                sponsorshipRequest.SponsorshipStartDate,
                sponsorshipRequest.SponsorshipEndDate,
                sponsorshipRequest.Status,
                sponsorshipRequest.CreatedAt,
                sponsorshipRequest.UpdatedAt,
                sponsorshipRequest.SubmittedAt,
                sponsorshipRequest.ApprovedAt,
                sponsorshipRequest.RejectedAt,
                sponsorshipRequest.CurrentApproverId,
                sponsorshipRequest.CurrentApproverName,
                sponsorshipRequest.FinalDecisionById,
                sponsorshipRequest.FinalDecisionByName,
                sponsorshipRequest.DecisionNotes,
                sponsorshipRequest.Attachments
                    .OrderByDescending(attachment => attachment.UploadedAt)
                    .Select(attachment => new RequestAttachmentDto(
                        attachment.Id,
                        attachment.FileName,
                        attachment.ContentType,
                        attachment.FileSizeBytes,
                        attachment.UploadedByName,
                        attachment.UploadedAt))
                    .ToArray()))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
