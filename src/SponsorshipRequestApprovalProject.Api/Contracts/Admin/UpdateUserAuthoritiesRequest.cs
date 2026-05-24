using System.ComponentModel.DataAnnotations;

namespace SponsorshipRequestApprovalProject.Api.Contracts.Admin;

public record UpdateUserAuthoritiesRequest(
    [param: Required] string FirstName,
    [param: Required] string LastName,
    string? Department,
    [param: Required] string Role);
