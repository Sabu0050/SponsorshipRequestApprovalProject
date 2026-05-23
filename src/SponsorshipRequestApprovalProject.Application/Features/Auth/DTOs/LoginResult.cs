namespace SponsorshipRequestApprovalProject.Application.Features.Auth.DTOs;

public record LoginResult(
    string AccessToken,
    DateTimeOffset ExpiresAt,
    string UserId,
    string Email,
    IReadOnlyCollection<string> Roles);
