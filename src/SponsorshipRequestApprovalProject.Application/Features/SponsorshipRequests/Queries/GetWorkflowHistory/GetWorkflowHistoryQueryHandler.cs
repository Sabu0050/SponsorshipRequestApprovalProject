using Microsoft.EntityFrameworkCore;
using SponsorshipRequestApprovalProject.Application.Common.CQRS;
using SponsorshipRequestApprovalProject.Application.Common.Interfaces;
using SponsorshipRequestApprovalProject.Application.Common.Models;
using SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.DTOs;

namespace SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.Queries.GetWorkflowHistory;

public class GetWorkflowHistoryQueryHandler(IApplicationDbContext dbContext)
    : IQueryHandler<GetWorkflowHistoryQuery, PagedResult<WorkflowHistoryDto>>
{
    public async Task<PagedResult<WorkflowHistoryDto>> Handle(
        GetWorkflowHistoryQuery request,
        CancellationToken cancellationToken)
    {
        var pageNumber = Math.Max(request.PageNumber, 1);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var query = dbContext.WorkflowHistories
            .AsNoTracking()
            .Where(history => history.SponsorshipRequestId == request.SponsorshipRequestId);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(history => history.PerformedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(history => new WorkflowHistoryDto(
                history.Id,
                history.SponsorshipRequestId,
                history.Action,
                history.FromStatus,
                history.ToStatus,
                history.PerformedById,
                history.PerformedByName,
                history.AssignedToId,
                history.AssignedToName,
                history.Remarks,
                history.PerformedAt))
            .ToListAsync(cancellationToken);

        return new PagedResult<WorkflowHistoryDto>(
            items,
            pageNumber,
            pageSize,
            totalCount);
    }
}
