using SponsorshipRequestApprovalProject.Domain.Enums;

namespace SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.DTOs;

public record WorkflowHistoryDto(
    Guid Id,
    Guid SponsorshipRequestId,
    WorkflowAction Action,
    SponsorshipRequestStatus? FromStatus,
    SponsorshipRequestStatus ToStatus,
    string PerformedById,
    string PerformedByName,
    string? AssignedToId,
    string? AssignedToName,
    string? Comments,
    DateTimeOffset PerformedAt);
