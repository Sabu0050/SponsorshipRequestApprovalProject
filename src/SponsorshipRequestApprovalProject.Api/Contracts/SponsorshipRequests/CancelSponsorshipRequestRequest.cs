using SponsorshipRequestApprovalProject.Domain.Enums;

namespace SponsorshipRequestApprovalProject.Api.Contracts.SponsorshipRequests;

public record CancelSponsorshipRequestRequest(
    SponsorshipRequestStatus ExpectedCurrentStatus,
    string? Comments);
