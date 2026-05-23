using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SponsorshipRequestApprovalProject.Application.Features.SponsorshipTypes.DTOs;
using SponsorshipRequestApprovalProject.Application.Features.SponsorshipTypes.Queries.GetSponsorshipTypeById;
using SponsorshipRequestApprovalProject.Application.Features.SponsorshipTypes.Queries.GetSponsorshipTypes;

namespace SponsorshipRequestApprovalProject.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/sponsorship-types")]
public class SponsorshipTypesController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<SponsorshipTypeDto>>> GetSponsorshipTypes(
        [FromQuery] bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new GetSponsorshipTypesQuery(isActive), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<SponsorshipTypeDto>> GetSponsorshipType(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetSponsorshipTypeByIdQuery(id), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }
}
