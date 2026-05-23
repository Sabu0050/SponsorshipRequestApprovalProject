namespace SponsorshipRequestApprovalProject.Domain.Enums;

public enum SponsorshipRequestStatus
{
    Draft = 0,
    PendingManagerApproval = 1,
    PendingFinanceReview = 2,
    Approved = 4,
    Rejected = 5,
    Cancelled = 7
}
