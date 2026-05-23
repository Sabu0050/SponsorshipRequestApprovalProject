using FluentValidation;

namespace SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.Queries.GetWorkflowHistory;

public class GetWorkflowHistoryQueryValidator : AbstractValidator<GetWorkflowHistoryQuery>
{
    public GetWorkflowHistoryQueryValidator()
    {
        RuleFor(query => query.SponsorshipRequestId)
            .NotEmpty();

        RuleFor(query => query.PageNumber)
            .GreaterThanOrEqualTo(1);

        RuleFor(query => query.PageSize)
            .InclusiveBetween(1, 100);
    }
}
