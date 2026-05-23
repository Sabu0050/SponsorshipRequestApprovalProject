using SponsorshipRequestApprovalProject.Application.Common.CQRS;
using SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.Commands.Common;
using SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.DTOs;
using SponsorshipRequestApprovalProject.Domain.Enums;

namespace SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.Commands.CancelSponsorshipRequest;

public class CancelSponsorshipRequestCommandHandler(WorkflowTransitionService workflowTransitionService)
    : ICommandHandler<CancelSponsorshipRequestCommand, SponsorshipRequestWorkflowResult>
{
    public Task<SponsorshipRequestWorkflowResult> Handle(
        CancelSponsorshipRequestCommand request,
        CancellationToken cancellationToken)
    {
        return workflowTransitionService.TransitionAsync(
            request.SponsorshipRequestId,
            request.ExpectedCurrentStatus,
            SponsorshipRequestStatus.Cancelled,
            WorkflowAction.Cancelled,
            request.PerformedById,
            request.PerformedByName,
            request.Comments,
            assignedToId: null,
            assignedToName: null,
            cancellationToken);
    }
}
