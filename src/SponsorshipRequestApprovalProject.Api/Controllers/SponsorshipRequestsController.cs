using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SponsorshipRequestApprovalProject.Api.Contracts.SponsorshipRequests;
using SponsorshipRequestApprovalProject.Application.Common.Identity;
using SponsorshipRequestApprovalProject.Application.Common.Models;
using SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.Commands.CancelSponsorshipRequest;
using SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.Commands.CreateSponsorshipRequest;
using SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.Commands.FinanceApproveSponsorshipRequest;
using SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.Commands.ManagerApproveSponsorshipRequest;
using SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.Commands.RejectSponsorshipRequest;
using SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.Commands.SubmitSponsorshipRequest;
using SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.Commands.UpdateSponsorshipRequest;
using SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.DTOs;
using SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.Queries.GetSponsorshipRequestById;
using SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.Queries.GetSponsorshipRequests;
using SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.Queries.GetWorkflowHistory;
using SponsorshipRequestApprovalProject.Domain.Enums;

namespace SponsorshipRequestApprovalProject.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/sponsorship-requests")]
public class SponsorshipRequestsController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<CreateSponsorshipRequestResult>> CreateSponsorshipRequest(
        CreateSponsorshipRequestRequest request,
        CancellationToken cancellationToken)
    {
        var createErrors = ValidateCreateOrUpdateRequest(request.Title, request.Description, request.SponsorName, request.CurrencyCode, request.SponsorshipTypeId);
        if (createErrors.Count > 0)
        {
            return ValidationProblem(new ValidationProblemDetails(createErrors)
            {
                Title = "Please correct the highlighted fields and try again."
            });
        }

        var result = await sender.Send(
            new CreateSponsorshipRequestCommand(
                request.Title.Trim(),
                request.Description.Trim(),
                request.SponsorshipTypeId,
                request.SponsorName.Trim(),
                request.RequestedAmount,
                request.CurrencyCode.Trim(),
                request.EventDate,
                request.SponsorshipStartDate,
                request.SponsorshipEndDate,
                GetCurrentUserId(),
                GetCurrentUserName(),
                GetCurrentUserEmail()),
            cancellationToken);

        return CreatedAtAction(
            nameof(GetSponsorshipRequest),
            new { id = result.Id },
            result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CreateSponsorshipRequestResult>> UpdateSponsorshipRequest(
        Guid id,
        UpdateSponsorshipRequestRequest request,
        CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
        {
            return ValidationProblem(new ValidationProblemDetails(new Dictionary<string, string[]>
            {
                { nameof(id), ["Request id is required."] }
            })
            {
                Title = "Please correct the highlighted fields and try again."
            });
        }

        var updateErrors = ValidateCreateOrUpdateRequest(request.Title, request.Description, request.SponsorName, request.CurrencyCode, request.SponsorshipTypeId);
        if (updateErrors.Count > 0)
        {
            return ValidationProblem(new ValidationProblemDetails(updateErrors)
            {
                Title = "Please correct the highlighted fields and try again."
            });
        }

        var result = await sender.Send(
            new UpdateSponsorshipRequestCommand(
                id,
                request.Title.Trim(),
                request.Description.Trim(),
                request.SponsorshipTypeId,
                request.SponsorName.Trim(),
                request.RequestedAmount,
                request.CurrencyCode.Trim(),
                request.EventDate,
                request.SponsorshipStartDate,
                request.SponsorshipEndDate,
                GetCurrentUserId()),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<SponsorshipRequestListItemDto>>> GetSponsorshipRequests(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] SponsorshipRequestStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(
            new GetSponsorshipRequestsQuery(
                pageNumber,
                pageSize,
                status,
                GetCurrentUserId(),
                User.IsInRole(ApplicationRoles.SystemAdmin),
                HasApprovalAuthority(ApprovalStages.Manager),
                HasApprovalAuthority(ApprovalStages.Finance)),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<SponsorshipRequestDetailDto>> GetSponsorshipRequest(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetSponsorshipRequestByIdQuery(id), cancellationToken);
        return result is null
            ? NotFound(new
            {
                title = "Sponsorship request not found.",
                detail = "The requested sponsorship request could not be located."
            })
            : Ok(result);
    }

    [HttpGet("{id:guid}/workflow-history")]
    public async Task<ActionResult<PagedResult<WorkflowHistoryDto>>> GetWorkflowHistory(
        Guid id,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(
            new GetWorkflowHistoryQuery(id, pageNumber, pageSize),
            cancellationToken);

        return Ok(result);
    }

    [HttpPost("{id:guid}/submissions")]
    public async Task<ActionResult<SponsorshipRequestWorkflowResult>> Submit(
        Guid id,
        SubmitSponsorshipRequestRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new SubmitSponsorshipRequestCommand(
                id,
                GetCurrentUserId(),
                GetCurrentUserName(),
                request.AssignedManagerId,
                request.AssignedManagerName,
                request.Comments),
            cancellationToken);

        return Ok(result);
    }

    [HttpPost("{id:guid}/manager-approvals")]
    public async Task<ActionResult<SponsorshipRequestWorkflowResult>> ManagerApprove(
        Guid id,
        ManagerApprovalRequest request,
        CancellationToken cancellationToken)
    {
        if (!HasApprovalAuthority(ApprovalStages.Manager))
        {
            return Forbid();
        }

        var result = await sender.Send(
            new ManagerApproveSponsorshipRequestCommand(
                id,
                GetCurrentUserId(),
                GetCurrentUserName(),
                request.AssignedFinanceReviewerId,
                request.AssignedFinanceReviewerName,
                request.Comments),
            cancellationToken);

        return Ok(result);
    }

    [HttpPost("{id:guid}/finance-approvals")]
    public async Task<ActionResult<SponsorshipRequestWorkflowResult>> FinanceApprove(
        Guid id,
        FinanceApprovalRequest request,
        CancellationToken cancellationToken)
    {
        if (!HasApprovalAuthority(ApprovalStages.Finance))
        {
            return Forbid();
        }

        var result = await sender.Send(
            new FinanceApproveSponsorshipRequestCommand(
                id,
                GetCurrentUserId(),
                GetCurrentUserName(),
                request.Comments),
            cancellationToken);

        return Ok(result);
    }

    [HttpPost("{id:guid}/rejections")]
    public async Task<ActionResult<SponsorshipRequestWorkflowResult>> Reject(
        Guid id,
        RejectSponsorshipRequestRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new RejectSponsorshipRequestCommand(
                id,
                request.ExpectedCurrentStatus,
                GetCurrentUserId(),
                GetCurrentUserName(),
                request.Comments),
            cancellationToken);

        return Ok(result);
    }

    [HttpPost("{id:guid}/cancellations")]
    public async Task<ActionResult<SponsorshipRequestWorkflowResult>> Cancel(
        Guid id,
        CancelSponsorshipRequestRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CancelSponsorshipRequestCommand(
                id,
                request.ExpectedCurrentStatus,
                GetCurrentUserId(),
                GetCurrentUserName(),
                request.Comments),
            cancellationToken);

        return Ok(result);
    }

    private string GetCurrentUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub")
            ?? string.Empty;
    }

    private string GetCurrentUserName()
    {
        return User.Identity?.Name
            ?? User.FindFirstValue(ClaimTypes.Email)
            ?? GetCurrentUserId();
    }

    private string GetCurrentUserEmail()
    {
        return User.FindFirstValue(ClaimTypes.Email)
            ?? User.FindFirstValue("email")
            ?? string.Empty;
    }

    private bool HasApprovalAuthority(string stage)
    {
        if (User.IsInRole(ApplicationRoles.SystemAdmin))
        {
            return true;
        }

        return User.Claims.Any(claim => claim.Type == ApprovalStages.ClaimType && claim.Value == stage);
    }

    private static Dictionary<string, string[]> ValidateCreateOrUpdateRequest(
        string? title,
        string? description,
        string? sponsorName,
        string? currencyCode,
        Guid sponsorshipTypeId)
    {
        var errors = new Dictionary<string, string[]>();
        if (string.IsNullOrWhiteSpace(title))
        {
            errors[nameof(title)] = ["Title is required."];
        }
        if (string.IsNullOrWhiteSpace(description))
        {
            errors[nameof(description)] = ["Description is required."];
        }
        if (string.IsNullOrWhiteSpace(sponsorName))
        {
            errors[nameof(sponsorName)] = ["Sponsor name is required."];
        }
        if (string.IsNullOrWhiteSpace(currencyCode))
        {
            errors[nameof(currencyCode)] = ["Currency code is required."];
        }
        if (sponsorshipTypeId == Guid.Empty)
        {
            errors[nameof(sponsorshipTypeId)] = ["Sponsorship type is required."];
        }

        return errors;
    }
}
