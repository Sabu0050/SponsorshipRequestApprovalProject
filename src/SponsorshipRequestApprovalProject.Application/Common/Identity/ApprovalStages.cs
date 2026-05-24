namespace SponsorshipRequestApprovalProject.Application.Common.Identity;

public static class ApprovalStages
{
    public const string ClaimType = "approval_stage";
    public const string AccessClaimType = "access_scope";
    public const string Requestor = "RequestorAccess";
    public const string Manager = "ManagerApproval";
    public const string Finance = "FinanceApproval";

    public static readonly string[] All = [Requestor, Manager, Finance];
}
