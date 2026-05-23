using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SponsorshipRequestApprovalProject.Api.Contracts.SponsorshipRequests;
using SponsorshipRequestApprovalProject.Application.Common.Models;
using SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.Commands.CancelSponsorshipRequest;
using SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.Commands.FinanceApproveSponsorshipRequest;
using SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.Commands.ManagerApproveSponsorshipRequest;
using SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.Commands.RejectSponsorshipRequest;
using SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.Commands.SubmitSponsorshipRequest;
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
                GetCurrentUserRole()),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<SponsorshipRequestDetailDto>> GetSponsorshipRequest(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetSponsorshipRequestByIdQuery(id), cancellationToken);
        return result is null ? NotFound() : Ok(result);
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

    private string? GetCurrentUserRole()
    {
        return User.FindFirstValue(ClaimTypes.Role);
    }
}
