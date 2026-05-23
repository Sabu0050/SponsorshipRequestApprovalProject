using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SponsorshipRequestApprovalProject.Application.Common.Identity;
using SponsorshipRequestApprovalProject.Application.Features.Admin.DTOs;
using SponsorshipRequestApprovalProject.Application.Features.Admin.Queries.GetRoles;

namespace SponsorshipRequestApprovalProject.Api.Controllers;

[ApiController]
[Authorize(Roles = ApplicationRoles.SystemAdmin)]
[Route("api/admin")]
public class AdminController(ISender sender) : ControllerBase
{
    [HttpGet("roles")]
    public async Task<ActionResult<IReadOnlyCollection<RoleDto>>> GetRoles(
        CancellationToken cancellationToken)
    {
        var roles = await sender.Send(new GetRolesQuery(), cancellationToken);
        return Ok(roles);
    }
}
