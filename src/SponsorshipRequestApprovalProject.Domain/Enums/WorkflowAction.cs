namespace SponsorshipRequestApprovalProject.Domain.Enums;

public enum WorkflowAction
{
    Created = 0,
    Submitted = 1,
    Assigned = 2,
    Reviewed = 3,
    Approved = 4,
    Rejected = 5,
    ReturnedForChanges = 6,
    Cancelled = 7
}
