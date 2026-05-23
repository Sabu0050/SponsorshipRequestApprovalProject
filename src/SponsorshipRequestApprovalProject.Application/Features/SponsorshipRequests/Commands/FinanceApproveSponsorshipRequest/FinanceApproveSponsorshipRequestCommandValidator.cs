using FluentValidation;
using SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.Commands.Common;

namespace SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.Commands.FinanceApproveSponsorshipRequest;

public class FinanceApproveSponsorshipRequestCommandValidator : AbstractValidator<FinanceApproveSponsorshipRequestCommand>
{
    public FinanceApproveSponsorshipRequestCommandValidator()
    {
        RuleFor(command => command.SponsorshipRequestId).ValidRequestId();
        RuleFor(command => command.PerformedById).ValidActorId();
        RuleFor(command => command.PerformedByName).ValidActorName();
        RuleFor(command => command.Comments).MaximumLength(2000);
    }
}
