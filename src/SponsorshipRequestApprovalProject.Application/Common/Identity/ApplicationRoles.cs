namespace SponsorshipRequestApprovalProject.Application.Common.Identity;

public static class ApplicationRoles
{
    public const string Requestor = nameof(Requestor);
    public const string Manager = nameof(Manager);
    public const string FinanceAdmin = nameof(FinanceAdmin);
    public const string SystemAdmin = nameof(SystemAdmin);

    public static readonly string[] All =
    [
        Requestor,
        Manager,
        FinanceAdmin,
        SystemAdmin
    ];
}
