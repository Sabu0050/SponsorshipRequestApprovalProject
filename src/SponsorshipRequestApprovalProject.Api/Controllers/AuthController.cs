using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using SponsorshipRequestApprovalProject.Api.Contracts.Auth;
using SponsorshipRequestApprovalProject.Application.Features.Auth.Commands.Login;
using SponsorshipRequestApprovalProject.Application.Features.Auth.DTOs;

namespace SponsorshipRequestApprovalProject.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(ISender sender) : ControllerBase
{
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
