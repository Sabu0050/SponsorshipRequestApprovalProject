namespace SponsorshipRequestApprovalProject.Application.Common.Exceptions;

public class WorkflowTransitionException(string message) : InvalidOperationException(message)
{
}
