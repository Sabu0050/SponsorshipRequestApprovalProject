using System.ComponentModel.DataAnnotations;
using SponsorshipRequestApprovalProject.Domain.Enums;

namespace SponsorshipRequestApprovalProject.Api.Contracts.SponsorshipRequests;

public record RejectSponsorshipRequestRequest(
    SponsorshipRequestStatus ExpectedCurrentStatus,
    [param: Required] string Comments);
