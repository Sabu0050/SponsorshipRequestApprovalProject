namespace SponsorshipRequestApprovalProject.Api.Contracts.SponsorshipTypes;

public record CreateSponsorshipTypeRequest(
    string Name,
    string? Description,
    bool IsActive = true);
