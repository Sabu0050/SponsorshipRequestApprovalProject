namespace SponsorshipRequestApprovalProject.Application.Common.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(
        string userId,
        string? email,
        IEnumerable<string> roles,
        IEnumerable<System.Security.Claims.Claim>? additionalClaims = null);
}
