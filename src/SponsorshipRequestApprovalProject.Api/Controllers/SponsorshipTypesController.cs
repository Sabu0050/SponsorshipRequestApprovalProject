using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SponsorshipRequestApprovalProject.Api.Contracts.SponsorshipTypes;
using SponsorshipRequestApprovalProject.Application.Common.Identity;
using SponsorshipRequestApprovalProject.Application.Features.SponsorshipTypes.DTOs;
using SponsorshipRequestApprovalProject.Application.Features.SponsorshipTypes.Queries.GetSponsorshipTypeById;
using SponsorshipRequestApprovalProject.Application.Features.SponsorshipTypes.Queries.GetSponsorshipTypes;
using SponsorshipRequestApprovalProject.Domain.Entities;
using SponsorshipRequestApprovalProject.Infrastructure.Persistence;

namespace SponsorshipRequestApprovalProject.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/sponsorship-types")]
public class SponsorshipTypesController(
    ISender sender,
    ApplicationDbContext dbContext) : ControllerBase
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

    [HttpPost]
    [Authorize(Roles = ApplicationRoles.SystemAdmin)]
    public async Task<ActionResult<SponsorshipTypeDto>> CreateSponsorshipType(
        CreateSponsorshipTypeRequest request,
        CancellationToken cancellationToken)
    {
        var name = request.Name.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            return ValidationProblem(new ValidationProblemDetails(new Dictionary<string, string[]>
            {
                { nameof(request.Name), ["Sponsorship type name is required."] }
            })
            {
                Title = "Please correct the highlighted fields and try again."
            });
        }

        var exists = await dbContext.SponsorshipTypes
            .AnyAsync(type => type.Name.ToLower() == name.ToLower(), cancellationToken);
        if (exists)
        {
            return Conflict(new
            {
                title = "Sponsorship type already exists.",
                detail = $"A sponsorship type with name '{name}' already exists."
            });
        }

        var entity = new SponsorshipType
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = request.Description?.Trim(),
            IsActive = request.IsActive
        };

        dbContext.SponsorshipTypes.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Ok(new SponsorshipTypeDto(
            entity.Id,
            entity.Name,
            entity.Description,
            entity.IsActive,
            entity.CreatedAt,
            entity.UpdatedAt));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = ApplicationRoles.SystemAdmin)]
    public async Task<ActionResult<SponsorshipTypeDto>> UpdateSponsorshipType(
        Guid id,
        UpdateSponsorshipTypeRequest request,
        CancellationToken cancellationToken)
    {
        var entity = await dbContext.SponsorshipTypes
            .FirstOrDefaultAsync(type => type.Id == id, cancellationToken);
        if (entity is null)
        {
            return NotFound(new
            {
                title = "Sponsorship type not found.",
                detail = "The requested sponsorship type could not be located."
            });
        }

        var name = request.Name.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            return ValidationProblem(new ValidationProblemDetails(new Dictionary<string, string[]>
            {
                { nameof(request.Name), ["Sponsorship type name is required."] }
            })
            {
                Title = "Please correct the highlighted fields and try again."
            });
        }

        var duplicate = await dbContext.SponsorshipTypes
            .AnyAsync(type => type.Id != id && type.Name.ToLower() == name.ToLower(), cancellationToken);
        if (duplicate)
        {
            return Conflict(new
            {
                title = "Sponsorship type already exists.",
                detail = $"A sponsorship type with name '{name}' already exists."
            });
        }

        entity.Name = name;
        entity.Description = request.Description?.Trim();
        entity.IsActive = request.IsActive;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Ok(new SponsorshipTypeDto(
            entity.Id,
            entity.Name,
            entity.Description,
            entity.IsActive,
            entity.CreatedAt,
            entity.UpdatedAt));
    }
}
