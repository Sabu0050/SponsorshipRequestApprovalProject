using Microsoft.EntityFrameworkCore;
using SponsorshipRequestApprovalProject.Application.Common.CQRS;
using SponsorshipRequestApprovalProject.Application.Common.Identity;
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

        query = ApplyRoleFilter(query, request.CurrentUserId, request.CurrentUserRole);

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

    private static IQueryable<SponsorshipRequest> ApplyRoleFilter(
        IQueryable<SponsorshipRequest> query,
        string? currentUserId,
        string? currentUserRole)
    {
        return currentUserRole switch
        {
            ApplicationRoles.Requestor when !string.IsNullOrWhiteSpace(currentUserId) =>
                query.Where(request => request.RequesterId == currentUserId),

            ApplicationRoles.Manager when !string.IsNullOrWhiteSpace(currentUserId) =>
                query.Where(request =>
                    request.Status == SponsorshipRequestStatus.PendingManagerApproval
                    && request.CurrentApproverId == currentUserId),

            ApplicationRoles.FinanceAdmin when !string.IsNullOrWhiteSpace(currentUserId) =>
                query.Where(request =>
                    request.Status == SponsorshipRequestStatus.PendingFinanceReview
                    && (request.CurrentApproverId == currentUserId || request.CurrentApproverId == null)),

            ApplicationRoles.SystemAdmin => query,
            _ => query
        };
    }
}
