using SponsorshipRequestApprovalProject.Domain.Enums;

namespace SponsorshipRequestApprovalProject.Domain.Entities;

public class SponsorshipRequest
{
    public Guid Id { get; set; }

    public string RequestNumber { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public Guid SponsorshipTypeId { get; set; }

    public SponsorshipType? SponsorshipType { get; set; }

    public string RequesterId { get; set; } = string.Empty;

    public string RequesterName { get; set; } = string.Empty;

    public string RequesterEmail { get; set; } = string.Empty;

    public string SponsorName { get; set; } = string.Empty;

    public decimal RequestedAmount { get; set; }

    public string CurrencyCode { get; set; } = string.Empty;

    public DateOnly? EventDate { get; set; }

    public DateOnly? SponsorshipStartDate { get; set; }

    public DateOnly? SponsorshipEndDate { get; set; }

    public SponsorshipRequestStatus Status { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }

    public DateTimeOffset? SubmittedAt { get; set; }

    public DateTimeOffset? ApprovedAt { get; set; }

    public DateTimeOffset? RejectedAt { get; set; }

    public string? CurrentApproverId { get; set; }

    public string? CurrentApproverName { get; set; }

    public string? FinalDecisionById { get; set; }

    public string? FinalDecisionByName { get; set; }

    public string? DecisionNotes { get; set; }

    public ICollection<WorkflowHistory> WorkflowHistories { get; set; } = [];

    public ICollection<RequestAttachment> Attachments { get; set; } = [];
}
