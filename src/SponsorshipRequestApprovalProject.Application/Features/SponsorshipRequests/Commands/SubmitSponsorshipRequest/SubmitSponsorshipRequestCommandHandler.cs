using SponsorshipRequestApprovalProject.Application.Common.CQRS;
using SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.Commands.Common;
using SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.DTOs;
using SponsorshipRequestApprovalProject.Domain.Enums;

namespace SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.Commands.SubmitSponsorshipRequest;

public class SubmitSponsorshipRequestCommandHandler(WorkflowTransitionService workflowTransitionService)
    : ICommandHandler<SubmitSponsorshipRequestCommand, SponsorshipRequestWorkflowResult>
{
    public Task<SponsorshipRequestWorkflowResult> Handle(
        SubmitSponsorshipRequestCommand request,
        CancellationToken cancellationToken)
    {
        return workflowTransitionService.TransitionAsync(
            request.SponsorshipRequestId,
            SponsorshipRequestStatus.Draft,
            SponsorshipRequestStatus.PendingManagerApproval,
            WorkflowAction.Submitted,
            request.PerformedById,
            request.PerformedByName,
            request.Comments,
            request.AssignedManagerId,
            request.AssignedManagerName,
            cancellationToken);
    }
}
