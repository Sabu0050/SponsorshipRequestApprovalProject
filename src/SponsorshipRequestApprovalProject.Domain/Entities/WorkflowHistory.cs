using SponsorshipRequestApprovalProject.Domain.Enums;

namespace SponsorshipRequestApprovalProject.Domain.Entities;

public class WorkflowHistory
{
    public Guid Id { get; set; }

    public Guid SponsorshipRequestId { get; set; }

    public SponsorshipRequest? SponsorshipRequest { get; set; }

    public WorkflowAction Action { get; set; }

    public SponsorshipRequestStatus? FromStatus { get; set; }

    public SponsorshipRequestStatus ToStatus { get; set; }

    public string PerformedById { get; set; } = string.Empty;

    public string PerformedByName { get; set; } = string.Empty;

    public string? AssignedToId { get; set; }

    public string? AssignedToName { get; set; }

    public string? Comments { get; set; }

    public DateTimeOffset PerformedAt { get; set; }
}
