using SponsorshipRequestApprovalProject.Application.Common.CQRS;
using SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.Commands.Common;
using SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.DTOs;
using SponsorshipRequestApprovalProject.Domain.Enums;

namespace SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.Commands.ManagerApproveSponsorshipRequest;

public class ManagerApproveSponsorshipRequestCommandHandler(WorkflowTransitionService workflowTransitionService)
    : ICommandHandler<ManagerApproveSponsorshipRequestCommand, SponsorshipRequestWorkflowResult>
{
    public Task<SponsorshipRequestWorkflowResult> Handle(
        ManagerApproveSponsorshipRequestCommand request,
        CancellationToken cancellationToken)
    {
        return workflowTransitionService.TransitionAsync(
            request.SponsorshipRequestId,
            SponsorshipRequestStatus.PendingManagerApproval,
            SponsorshipRequestStatus.PendingFinanceReview,
            WorkflowAction.ManagerApproved,
            request.PerformedById,
            request.PerformedByName,
            request.Comments,
            request.AssignedFinanceReviewerId,
            request.AssignedFinanceReviewerName,
            cancellationToken);
    }
}
