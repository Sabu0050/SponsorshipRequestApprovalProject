using SponsorshipRequestApprovalProject.Domain.Enums;

namespace SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.DTOs;

public record SponsorshipRequestWorkflowResult(
    Guid SponsorshipRequestId,
    SponsorshipRequestStatus Status,
    WorkflowAction Action,
    DateTimeOffset PerformedAt);
