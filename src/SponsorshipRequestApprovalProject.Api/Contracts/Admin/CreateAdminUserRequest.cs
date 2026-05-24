namespace SponsorshipRequestApprovalProject.Api.Contracts.Admin;

public record CreateAdminUserRequest(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string? Department,
    string Role);
