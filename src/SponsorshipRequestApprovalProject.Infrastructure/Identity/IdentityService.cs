using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SponsorshipRequestApprovalProject.Application.Common.Interfaces;
using SponsorshipRequestApprovalProject.Application.Features.Auth.DTOs;

namespace SponsorshipRequestApprovalProject.Infrastructure.Identity;

public class IdentityService(
    UserManager<ApplicationUser> userManager,
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
        var accessToken = jwtTokenService.GenerateToken(user.Id, user.Email, roles);
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(jwtOptions.Value.ExpirationMinutes);

        return new LoginResult(
            accessToken,
            expiresAt,
            user.Id,
            user.Email ?? string.Empty,
            roles.ToArray());
    }
}
