using FluentValidation;

namespace SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.Queries.GetSponsorshipRequests;

public class GetSponsorshipRequestsQueryValidator : AbstractValidator<GetSponsorshipRequestsQuery>
{
    public GetSponsorshipRequestsQueryValidator()
    {
        RuleFor(query => query.PageNumber)
            .GreaterThanOrEqualTo(1);

        RuleFor(query => query.PageSize)
            .InclusiveBetween(1, 100);

        RuleFor(query => query.CurrentUserId)
            .MaximumLength(450);
    }
}
