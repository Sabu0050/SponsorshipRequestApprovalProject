using SponsorshipRequestApprovalProject.Domain.Enums;

namespace SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.DTOs;

public record SponsorshipRequestListItemDto(
    Guid Id,
    string RequestNumber,
    string Title,
    string SponsorshipTypeName,
    string RequesterName,
    string SponsorName,
    decimal RequestedAmount,
    string CurrencyCode,
    SponsorshipRequestStatus Status,
    string? CurrentApproverId,
    string? CurrentApproverName,
    DateTimeOffset CreatedAt,
    DateTimeOffset? SubmittedAt,
    DateTimeOffset? ApprovedAt,
    DateTimeOffset? RejectedAt);
