using SponsorshipRequestApprovalProject.Application.Common.CQRS;
using SponsorshipRequestApprovalProject.Application.Common.Models;
using SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.DTOs;
using SponsorshipRequestApprovalProject.Domain.Enums;

namespace SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.Queries.GetSponsorshipRequests;

public record GetSponsorshipRequestsQuery(
    int PageNumber,
    int PageSize,
    SponsorshipRequestStatus? Status,
    string? CurrentUserId,
    bool IsSystemAdmin,
    bool HasManagerApprovalAuthority,
    bool HasFinanceApprovalAuthority) : IQuery<PagedResult<SponsorshipRequestListItemDto>>;
