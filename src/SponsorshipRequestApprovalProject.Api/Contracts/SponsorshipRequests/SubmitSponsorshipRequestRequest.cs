namespace SponsorshipRequestApprovalProject.Api.Contracts.SponsorshipRequests;

public record SubmitSponsorshipRequestRequest(
    string? AssignedManagerId,
    string? AssignedManagerName,
    string? Comments);
