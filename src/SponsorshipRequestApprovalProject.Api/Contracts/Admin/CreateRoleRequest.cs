using System.ComponentModel.DataAnnotations;

namespace SponsorshipRequestApprovalProject.Api.Contracts.Admin;

public record CreateRoleRequest(
    [param: Required, MinLength(1)] string Name,
    bool CanRequestorAccess,
    bool CanApproveManagerStage,
    bool CanApproveFinanceStage);
