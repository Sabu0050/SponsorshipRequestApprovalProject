using FluentValidation;

namespace SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.Commands.Common;

public static class WorkflowCommandValidatorExtensions
{
    public static IRuleBuilderOptions<T, Guid> ValidRequestId<T>(
        this IRuleBuilder<T, Guid> ruleBuilder)
    {
        return ruleBuilder.NotEmpty();
    }

    public static IRuleBuilderOptions<T, string> ValidActorId<T>(
        this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty()
            .MaximumLength(450);
    }

    public static IRuleBuilderOptions<T, string> ValidActorName<T>(
        this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty()
            .MaximumLength(200);
    }
}
