using SponsorshipRequestApprovalProject.Application.Common.CQRS;
using SponsorshipRequestApprovalProject.Application.Common.Models;
using SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.DTOs;

namespace SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.Queries.GetWorkflowHistory;

public record GetWorkflowHistoryQuery(
    Guid SponsorshipRequestId,
    int PageNumber,
    int PageSize) : IQuery<PagedResult<WorkflowHistoryDto>>;
