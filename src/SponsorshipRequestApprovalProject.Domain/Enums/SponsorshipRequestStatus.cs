namespace SponsorshipRequestApprovalProject.Domain.Enums;

public enum SponsorshipRequestStatus
{
    Draft = 0,
    Submitted = 1,
    InReview = 2,
    PendingApproval = 3,
    Approved = 4,
    Rejected = 5,
    ReturnedForChanges = 6,
    Cancelled = 7
}
