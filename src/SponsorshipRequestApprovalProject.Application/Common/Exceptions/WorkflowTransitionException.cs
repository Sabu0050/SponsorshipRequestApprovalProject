namespace SponsorshipRequestApprovalProject.Application.Common.Exceptions;

public class WorkflowTransitionException(string message) : BusinessRuleException(message)
{
}
