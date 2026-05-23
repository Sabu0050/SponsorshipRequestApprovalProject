using SponsorshipRequestApprovalProject.Domain.Enums;

namespace SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.DTOs;

public record SponsorshipRequestDetailDto(
    Guid Id,
    string RequestNumber,
    string Title,
    string Description,
    Guid SponsorshipTypeId,
    string SponsorshipTypeName,
    string RequesterId,
    string RequesterName,
    string RequesterEmail,
    string SponsorName,
    decimal RequestedAmount,
    string CurrencyCode,
    DateOnly? EventDate,
    DateOnly? SponsorshipStartDate,
    DateOnly? SponsorshipEndDate,
    SponsorshipRequestStatus Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    DateTimeOffset? SubmittedAt,
    DateTimeOffset? ApprovedAt,
    DateTimeOffset? RejectedAt,
    string? CurrentApproverId,
    string? CurrentApproverName,
    string? FinalDecisionById,
    string? FinalDecisionByName,
    string? DecisionNotes,
    IReadOnlyCollection<RequestAttachmentDto> Attachments);
