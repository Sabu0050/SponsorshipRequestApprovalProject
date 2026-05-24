using Microsoft.EntityFrameworkCore;
using SponsorshipRequestApprovalProject.Application.Common.CQRS;
using SponsorshipRequestApprovalProject.Application.Common.Interfaces;
using SponsorshipRequestApprovalProject.Application.Common.Models;
using SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.DTOs;
using SponsorshipRequestApprovalProject.Domain.Entities;
using SponsorshipRequestApprovalProject.Domain.Enums;

namespace SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.Queries.GetSponsorshipRequests;

public class GetSponsorshipRequestsQueryHandler(IApplicationDbContext dbContext)
    : IQueryHandler<GetSponsorshipRequestsQuery, PagedResult<SponsorshipRequestListItemDto>>
{
    public async Task<PagedResult<SponsorshipRequestListItemDto>> Handle(
        GetSponsorshipRequestsQuery request,
        CancellationToken cancellationToken)
    {
        var pageNumber = Math.Max(request.PageNumber, 1);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var query = dbContext.SponsorshipRequests
            .AsNoTracking()
            .AsQueryable();

        if (request.Status.HasValue)
        {
            query = query.Where(sponsorshipRequest => sponsorshipRequest.Status == request.Status.Value);
        }

        query = ApplyUserScopedFilter(
            query,
            request.CurrentUserId,
            request.Status,
            request.IsSystemAdmin,
            request.HasManagerApprovalAuthority,
            request.HasFinanceApprovalAuthority);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(sponsorshipRequest => sponsorshipRequest.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(sponsorshipRequest => new SponsorshipRequestListItemDto(
                sponsorshipRequest.Id,
                sponsorshipRequest.RequestNumber,
                sponsorshipRequest.Title,
                sponsorshipRequest.SponsorshipType == null ? string.Empty : sponsorshipRequest.SponsorshipType.Name,
                sponsorshipRequest.RequesterName,
                sponsorshipRequest.SponsorName,
                sponsorshipRequest.RequestedAmount,
                sponsorshipRequest.CurrencyCode,
                sponsorshipRequest.Status,
                sponsorshipRequest.CurrentApproverId,
                sponsorshipRequest.CurrentApproverName,
                sponsorshipRequest.CreatedAt,
                sponsorshipRequest.SubmittedAt,
                sponsorshipRequest.ApprovedAt,
                sponsorshipRequest.RejectedAt))
            .ToListAsync(cancellationToken);

        return new PagedResult<SponsorshipRequestListItemDto>(
            items,
            pageNumber,
            pageSize,
            totalCount);
    }

    private static IQueryable<SponsorshipRequest> ApplyUserScopedFilter(
        IQueryable<SponsorshipRequest> query,
        string? currentUserId,
        SponsorshipRequestStatus? requestedStatus,
        bool isSystemAdmin,
        bool hasManagerApprovalAuthority,
        bool hasFinanceApprovalAuthority)
    {
        if (isSystemAdmin)
        {
            return query;
        }

        if (string.IsNullOrWhiteSpace(currentUserId))
        {
            return query.Where(_ => false);
        }

        if (requestedStatus == SponsorshipRequestStatus.PendingManagerApproval && hasManagerApprovalAuthority)
        {
            return query.Where(request =>
                request.Status == SponsorshipRequestStatus.PendingManagerApproval
                && request.RequesterId != currentUserId
                && (string.IsNullOrWhiteSpace(request.CurrentApproverId) || request.CurrentApproverId == currentUserId));
        }

        if (requestedStatus == SponsorshipRequestStatus.PendingFinanceReview && hasFinanceApprovalAuthority)
        {
            return query.Where(request =>
                request.Status == SponsorshipRequestStatus.PendingFinanceReview
                && request.RequesterId != currentUserId
                && (string.IsNullOrWhiteSpace(request.CurrentApproverId) || request.CurrentApproverId == currentUserId));
        }

        // Personal decision report scope: only show items finalized by the current user.
        if (requestedStatus is SponsorshipRequestStatus.Approved or SponsorshipRequestStatus.Rejected)
        {
            return query.Where(request =>
                request.Status == requestedStatus
                && request.FinalDecisionById == currentUserId);
        }

        // Default user scope: requestors and non-admin users see only their own requests.
        return query.Where(request => request.RequesterId == currentUserId);
    }
}
