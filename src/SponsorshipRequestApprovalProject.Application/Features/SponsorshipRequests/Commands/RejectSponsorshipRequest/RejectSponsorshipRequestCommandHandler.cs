using SponsorshipRequestApprovalProject.Application.Common.CQRS;
using SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.Commands.Common;
using SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.DTOs;
using SponsorshipRequestApprovalProject.Domain.Enums;

namespace SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.Commands.RejectSponsorshipRequest;

public class RejectSponsorshipRequestCommandHandler(WorkflowTransitionService workflowTransitionService)
    : ICommandHandler<RejectSponsorshipRequestCommand, SponsorshipRequestWorkflowResult>
{
    public Task<SponsorshipRequestWorkflowResult> Handle(
        RejectSponsorshipRequestCommand request,
        CancellationToken cancellationToken)
    {
        return workflowTransitionService.TransitionAsync(
            request.SponsorshipRequestId,
            request.ExpectedCurrentStatus,
            SponsorshipRequestStatus.Rejected,
            WorkflowAction.Rejected,
            request.PerformedById,
            request.PerformedByName,
            request.Comments,
            assignedToId: null,
            assignedToName: null,
            cancellationToken);
    }
}
