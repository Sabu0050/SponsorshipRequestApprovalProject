using SponsorshipRequestApprovalProject.Domain.Enums;

namespace SponsorshipRequestApprovalProject.Api.Contracts.SponsorshipRequests;

public record RejectSponsorshipRequestRequest(
    SponsorshipRequestStatus ExpectedCurrentStatus,
    string Comments);
