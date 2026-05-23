using Microsoft.AspNetCore.Identity;

namespace SponsorshipRequestApprovalProject.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string? Department { get; set; }

    public bool IsActive { get; set; } = true;
}
