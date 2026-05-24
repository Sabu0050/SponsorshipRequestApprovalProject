namespace SponsorshipRequestApprovalProject.Application.Features.Auth.DTOs;

public record LoginResult(
    string AccessToken,
    DateTimeOffset ExpiresAt,
    string UserId,
    string Email,
    string FirstName,
    string LastName,
    string FullName,
    IReadOnlyCollection<string> Roles);
