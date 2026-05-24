namespace SponsorshipRequestApprovalProject.Api.Contracts.Admin;

public record UpdateRoleAuthoritiesRequest(
    bool CanRequestorAccess,
    bool CanApproveManagerStage,
    bool CanApproveFinanceStage);
