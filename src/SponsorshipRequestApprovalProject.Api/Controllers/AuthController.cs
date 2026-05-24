using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MediatR;
using SponsorshipRequestApprovalProject.Api.Contracts.Auth;
using SponsorshipRequestApprovalProject.Application.Features.Auth.Commands.Login;
using SponsorshipRequestApprovalProject.Application.Features.Auth.DTOs;
using SponsorshipRequestApprovalProject.Infrastructure.Identity;

namespace SponsorshipRequestApprovalProject.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(ISender sender) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet("login-options")]
    public async Task<ActionResult<IReadOnlyCollection<LoginOptionResponse>>> GetLoginOptions(
        [FromServices] RoleManager<IdentityRole> roleManager,
        [FromServices] UserManager<ApplicationUser> userManager,
        CancellationToken cancellationToken)
    {
        var roles = await roleManager.Roles
            .OrderBy(role => role.Name)
            .ToListAsync(cancellationToken);

        var options = new List<LoginOptionResponse>();
        foreach (var role in roles)
        {
            var roleName = role.Name ?? string.Empty;
            if (string.IsNullOrWhiteSpace(roleName))
            {
                continue;
            }

            var users = await userManager.GetUsersInRoleAsync(roleName);
            var email = users
                .Where(user => !string.IsNullOrWhiteSpace(user.Email))
                .Select(user => user.Email!)
                .OrderBy(value => value)
                .FirstOrDefault() ?? string.Empty;

            options.Add(new LoginOptionResponse(roleName, email));
        }

        return Ok(options);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<LoginResult>> Login(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new LoginCommand(request.Email, request.Password),
            cancellationToken);

        if (result is null)
        {
            return Unauthorized();
        }

        return Ok(result);
    }
}
