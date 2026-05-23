using FluentValidation;
using SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.Commands.Common;
using SponsorshipRequestApprovalProject.Domain.Enums;

namespace SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.Commands.CancelSponsorshipRequest;

public class CancelSponsorshipRequestCommandValidator : AbstractValidator<CancelSponsorshipRequestCommand>
{
    public CancelSponsorshipRequestCommandValidator()
    {
        RuleFor(command => command.SponsorshipRequestId).ValidRequestId();
        RuleFor(command => command.ExpectedCurrentStatus)
            .Must(status => status is SponsorshipRequestStatus.Draft
                or SponsorshipRequestStatus.PendingManagerApproval
                or SponsorshipRequestStatus.PendingFinanceReview);
        RuleFor(command => command.PerformedById).ValidActorId();
        RuleFor(command => command.PerformedByName).ValidActorName();
        RuleFor(command => command.Comments).MaximumLength(2000);
    }
}
