using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SponsorshipRequestApprovalProject.Application.Common.Identity;
using SponsorshipRequestApprovalProject.Application.Common.Interfaces;
using SponsorshipRequestApprovalProject.Application.Features.Auth.DTOs;

namespace SponsorshipRequestApprovalProject.Infrastructure.Identity;

public class IdentityService(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager,
    SignInManager<ApplicationUser> signInManager,
    IJwtTokenService jwtTokenService,
    IOptions<JwtOptions> jwtOptions) : IIdentityService
{
    public async Task<LoginResult?> LoginAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null || !user.IsActive)
        {
            return null;
        }

        var signInResult = await signInManager.CheckPasswordSignInAsync(
            user,
            password,
            lockoutOnFailure: true);

        if (!signInResult.Succeeded)
        {
            return null;
        }

        var roles = await userManager.GetRolesAsync(user);
        var userClaims = await userManager.GetClaimsAsync(user);
        var nonAuthorityUserClaims = userClaims.Where(claim =>
            claim.Type != ApprovalStages.ClaimType &&
            claim.Type != ApprovalStages.AccessClaimType).ToList();

        var roleAuthorityClaims = new List<System.Security.Claims.Claim>();
        foreach (var roleName in roles)
        {
            var role = await roleManager.FindByNameAsync(roleName);
            if (role is null)
            {
                continue;
            }

            var roleClaims = await roleManager.GetClaimsAsync(role);
            roleAuthorityClaims.AddRange(roleClaims.Where(claim =>
                claim.Type == ApprovalStages.ClaimType ||
                claim.Type == ApprovalStages.AccessClaimType));
        }

        var claims = nonAuthorityUserClaims
            .Concat(roleAuthorityClaims)
            .DistinctBy(claim => $"{claim.Type}:{claim.Value}")
            .ToArray();

        var accessToken = jwtTokenService.GenerateToken(user.Id, user.Email, roles, claims);
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(jwtOptions.Value.ExpirationMinutes);
        var firstName = user.FirstName?.Trim() ?? string.Empty;
        var lastName = user.LastName?.Trim() ?? string.Empty;
        var fullName = $"{firstName} {lastName}".Trim();

        return new LoginResult(
            accessToken,
            expiresAt,
            user.Id,
            user.Email ?? string.Empty,
            firstName,
            lastName,
            string.IsNullOrWhiteSpace(fullName) ? user.Email ?? string.Empty : fullName,
            roles.ToArray());
    }
}
