using FluentValidation;

namespace SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.Queries.GetSponsorshipRequestById;

public class GetSponsorshipRequestByIdQueryValidator : AbstractValidator<GetSponsorshipRequestByIdQuery>
{
    public GetSponsorshipRequestByIdQueryValidator()
    {
        RuleFor(query => query.SponsorshipRequestId)
            .NotEmpty();
    }
}
