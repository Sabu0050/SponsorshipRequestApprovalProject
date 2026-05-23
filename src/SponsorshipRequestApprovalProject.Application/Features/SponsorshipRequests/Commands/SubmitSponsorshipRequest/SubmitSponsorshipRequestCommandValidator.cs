using FluentValidation;
using SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.Commands.Common;

namespace SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.Commands.SubmitSponsorshipRequest;

public class SubmitSponsorshipRequestCommandValidator : AbstractValidator<SubmitSponsorshipRequestCommand>
{
    public SubmitSponsorshipRequestCommandValidator()
    {
        RuleFor(command => command.SponsorshipRequestId).ValidRequestId();
        RuleFor(command => command.PerformedById).ValidActorId();
        RuleFor(command => command.PerformedByName).ValidActorName();
        RuleFor(command => command.AssignedManagerId).MaximumLength(450);
        RuleFor(command => command.AssignedManagerName).MaximumLength(200);
        RuleFor(command => command.Comments).MaximumLength(2000);
    }
}
