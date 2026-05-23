using SponsorshipRequestApprovalProject.Application.Common.CQRS;
using SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.Commands.Common;
using SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.DTOs;
using SponsorshipRequestApprovalProject.Domain.Enums;

namespace SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.Commands.FinanceApproveSponsorshipRequest;

public class FinanceApproveSponsorshipRequestCommandHandler(WorkflowTransitionService workflowTransitionService)
    : ICommandHandler<FinanceApproveSponsorshipRequestCommand, SponsorshipRequestWorkflowResult>
{
    public Task<SponsorshipRequestWorkflowResult> Handle(
        FinanceApproveSponsorshipRequestCommand request,
        CancellationToken cancellationToken)
    {
        return workflowTransitionService.TransitionAsync(
            request.SponsorshipRequestId,
            SponsorshipRequestStatus.PendingFinanceReview,
            SponsorshipRequestStatus.Approved,
            WorkflowAction.FinanceApproved,
            request.PerformedById,
            request.PerformedByName,
            request.Comments,
            assignedToId: null,
            assignedToName: null,
            cancellationToken);
    }
}
