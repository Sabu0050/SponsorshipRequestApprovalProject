namespace SponsorshipRequestApprovalProject.Api.Contracts.SponsorshipRequests;

public record ManagerApprovalRequest(
    string? AssignedFinanceReviewerId,
    string? AssignedFinanceReviewerName,
    string? Comments);
