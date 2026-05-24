using System.ComponentModel.DataAnnotations;

namespace SponsorshipRequestApprovalProject.Api.Contracts.Admin;

public record CreateAdminUserRequest(
    [param: Required, EmailAddress] string Email,
    [param: Required, MinLength(8)] string Password,
    [param: Required] string FirstName,
    [param: Required] string LastName,
    string? Department,
    [param: Required] string Role);
