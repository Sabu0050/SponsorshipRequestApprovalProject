using FluentValidation;

namespace SponsorshipRequestApprovalProject.Application.Features.SponsorshipTypes.Queries.GetSponsorshipTypeById;

public class GetSponsorshipTypeByIdQueryValidator : AbstractValidator<GetSponsorshipTypeByIdQuery>
{
    public GetSponsorshipTypeByIdQueryValidator()
    {
        RuleFor(query => query.SponsorshipTypeId)
            .NotEmpty();
    }
}
