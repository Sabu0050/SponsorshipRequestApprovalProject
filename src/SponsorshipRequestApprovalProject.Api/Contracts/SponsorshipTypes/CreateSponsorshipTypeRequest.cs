using System.ComponentModel.DataAnnotations;

namespace SponsorshipRequestApprovalProject.Api.Contracts.SponsorshipTypes;

public record CreateSponsorshipTypeRequest(
    [param: Required] string Name,
    string? Description,
    bool IsActive = true);
