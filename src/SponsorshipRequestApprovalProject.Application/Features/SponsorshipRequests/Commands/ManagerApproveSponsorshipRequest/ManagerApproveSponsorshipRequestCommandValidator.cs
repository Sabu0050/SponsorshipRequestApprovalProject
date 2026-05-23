using FluentValidation;
using SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.Commands.Common;

namespace SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.Commands.ManagerApproveSponsorshipRequest;

public class ManagerApproveSponsorshipRequestCommandValidator : AbstractValidator<ManagerApproveSponsorshipRequestCommand>
{
    public ManagerApproveSponsorshipRequestCommandValidator()
    {
        RuleFor(command => command.SponsorshipRequestId).ValidRequestId();
        RuleFor(command => command.PerformedById).ValidActorId();
        RuleFor(command => command.PerformedByName).ValidActorName();
        RuleFor(command => command.AssignedFinanceReviewerId).MaximumLength(450);
        RuleFor(command => command.AssignedFinanceReviewerName).MaximumLength(200);
        RuleFor(command => command.Comments).MaximumLength(2000);
    }
}
