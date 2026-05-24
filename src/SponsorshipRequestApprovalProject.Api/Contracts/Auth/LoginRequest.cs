using System.ComponentModel.DataAnnotations;

namespace SponsorshipRequestApprovalProject.Api.Contracts.Auth;

public record LoginRequest(
    [param: Required, EmailAddress] string Email,
    [param: Required] string Password);
