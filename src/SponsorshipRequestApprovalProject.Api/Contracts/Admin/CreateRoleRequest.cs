namespace SponsorshipRequestApprovalProject.Api.Contracts.Admin;

public record CreateRoleRequest(
    string Name,
    bool CanRequestorAccess,
    bool CanApproveManagerStage,
    bool CanApproveFinanceStage);
