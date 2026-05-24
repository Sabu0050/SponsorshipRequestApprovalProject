using System.ComponentModel.DataAnnotations;

namespace SponsorshipRequestApprovalProject.Api.Contracts.SponsorshipTypes;

public record UpdateSponsorshipTypeRequest(
    [param: Required] string Name,
    string? Description,
    bool IsActive);
