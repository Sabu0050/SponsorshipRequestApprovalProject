namespace SponsorshipRequestApprovalProject.Api.Contracts.Admin;

public record UpdateUserAuthoritiesRequest(
    string FirstName,
    string LastName,
    string? Department,
    string Role);
