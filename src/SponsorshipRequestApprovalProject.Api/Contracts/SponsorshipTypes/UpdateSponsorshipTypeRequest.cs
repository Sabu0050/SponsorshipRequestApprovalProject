namespace SponsorshipRequestApprovalProject.Api.Contracts.SponsorshipTypes;

public record UpdateSponsorshipTypeRequest(
    string Name,
    string? Description,
    bool IsActive);
