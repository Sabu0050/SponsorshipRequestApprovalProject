using SponsorshipRequestApprovalProject.Domain.Enums;

namespace SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.DTOs;

public record CreateSponsorshipRequestResult(
    Guid Id,
    string RequestNumber,
    SponsorshipRequestStatus Status);
