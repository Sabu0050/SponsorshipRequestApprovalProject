using FluentValidation;

namespace SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.Commands.UpdateSponsorshipRequest;

public class UpdateSponsorshipRequestCommandValidator : AbstractValidator<UpdateSponsorshipRequestCommand>
{
    public UpdateSponsorshipRequestCommandValidator()
    {
        RuleFor(command => command.SponsorshipRequestId)
            .NotEqual(Guid.Empty);

        RuleFor(command => command.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(command => command.Description)
            .NotEmpty()
            .MaximumLength(4000);

        RuleFor(command => command.SponsorshipTypeId)
            .NotEqual(Guid.Empty);

        RuleFor(command => command.SponsorName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(command => command.RequestedAmount)
            .GreaterThan(0);

        RuleFor(command => command.CurrencyCode)
            .NotEmpty()
            .Length(3);

        RuleFor(command => command.CurrentUserId)
            .NotEmpty();
    }
}
