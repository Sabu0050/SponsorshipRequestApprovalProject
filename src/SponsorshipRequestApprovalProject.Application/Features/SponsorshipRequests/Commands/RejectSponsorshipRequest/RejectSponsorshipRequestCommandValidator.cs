using FluentValidation;
using SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.Commands.Common;
using SponsorshipRequestApprovalProject.Domain.Enums;

namespace SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.Commands.RejectSponsorshipRequest;

public class RejectSponsorshipRequestCommandValidator : AbstractValidator<RejectSponsorshipRequestCommand>
{
    public RejectSponsorshipRequestCommandValidator()
    {
        RuleFor(command => command.SponsorshipRequestId).ValidRequestId();
        RuleFor(command => command.ExpectedCurrentStatus)
            .Must(status => status is SponsorshipRequestStatus.PendingManagerApproval
                or SponsorshipRequestStatus.PendingFinanceReview);
        RuleFor(command => command.PerformedById).ValidActorId();
        RuleFor(command => command.PerformedByName).ValidActorName();
        RuleFor(command => command.Comments)
            .NotEmpty()
            .MaximumLength(2000);
    }
}
