using Microsoft.EntityFrameworkCore;
using SponsorshipRequestApprovalProject.Application.Common.Exceptions;
using SponsorshipRequestApprovalProject.Application.Common.Interfaces;
using SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.DTOs;
using SponsorshipRequestApprovalProject.Domain.Entities;
using SponsorshipRequestApprovalProject.Domain.Enums;

namespace SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.Commands.Common;

public class WorkflowTransitionService(IApplicationDbContext dbContext)
{
    public async Task<SponsorshipRequestWorkflowResult> TransitionAsync(
        Guid sponsorshipRequestId,
        SponsorshipRequestStatus requiredStatus,
        SponsorshipRequestStatus targetStatus,
        WorkflowAction action,
        string performedById,
        string performedByName,
        string? comments,
        string? assignedToId,
        string? assignedToName,
        CancellationToken cancellationToken)
    {
        var sponsorshipRequest = await dbContext.SponsorshipRequests
            .FirstOrDefaultAsync(request => request.Id == sponsorshipRequestId, cancellationToken);

        if (sponsorshipRequest is null)
        {
            throw new WorkflowTransitionException("Sponsorship request was not found.");
        }

        EnsureNotTerminal(sponsorshipRequest.Status);

        if (sponsorshipRequest.Status != requiredStatus)
        {
            throw new WorkflowTransitionException(
                $"Cannot transition sponsorship request from {sponsorshipRequest.Status} to {targetStatus}.");
        }

        var fromStatus = sponsorshipRequest.Status;
        var performedAt = DateTimeOffset.UtcNow;

        sponsorshipRequest.Status = targetStatus;
        ApplyDecisionFields(
            sponsorshipRequest,
            targetStatus,
            performedById,
            performedByName,
            comments,
            assignedToId,
            assignedToName,
            performedAt);

        dbContext.WorkflowHistories.Add(new WorkflowHistory
        {
            Id = Guid.NewGuid(),
            SponsorshipRequestId = sponsorshipRequest.Id,
            Action = action,
            FromStatus = fromStatus,
            ToStatus = targetStatus,
            PerformedById = performedById,
            PerformedByName = performedByName,
            AssignedToId = assignedToId,
            AssignedToName = assignedToName,
            Remarks = comments,
            PerformedAt = performedAt
        });

        await dbContext.SaveChangesAsync(cancellationToken);

        return new SponsorshipRequestWorkflowResult(
            sponsorshipRequest.Id,
            sponsorshipRequest.Status,
            action,
            performedAt);
    }

    private static void EnsureNotTerminal(SponsorshipRequestStatus status)
    {
        if (status is SponsorshipRequestStatus.Rejected or SponsorshipRequestStatus.Cancelled)
        {
            throw new WorkflowTransitionException($"Sponsorship request is terminal in {status} status.");
        }
    }

    private static void ApplyDecisionFields(
        SponsorshipRequest sponsorshipRequest,
        SponsorshipRequestStatus targetStatus,
        string performedById,
        string performedByName,
        string? comments,
        string? assignedToId,
        string? assignedToName,
        DateTimeOffset performedAt)
    {
        sponsorshipRequest.CurrentApproverId = assignedToId;
        sponsorshipRequest.CurrentApproverName = assignedToName;

        if (targetStatus == SponsorshipRequestStatus.PendingManagerApproval)
        {
            sponsorshipRequest.SubmittedAt = performedAt;
        }

        if (targetStatus == SponsorshipRequestStatus.Approved)
        {
            sponsorshipRequest.ApprovedAt = performedAt;
        }

        if (targetStatus == SponsorshipRequestStatus.Rejected)
        {
            sponsorshipRequest.RejectedAt = performedAt;
        }

        if (targetStatus is SponsorshipRequestStatus.Approved or SponsorshipRequestStatus.Rejected)
        {
            sponsorshipRequest.FinalDecisionById = performedById;
            sponsorshipRequest.FinalDecisionByName = performedByName;
            sponsorshipRequest.DecisionNotes = comments;
            sponsorshipRequest.CurrentApproverId = null;
            sponsorshipRequest.CurrentApproverName = null;
        }
    }
}
