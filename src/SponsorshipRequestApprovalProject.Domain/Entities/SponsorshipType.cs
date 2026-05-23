namespace SponsorshipRequestApprovalProject.Domain.Entities;

public class SponsorshipType
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public ICollection<SponsorshipRequest> SponsorshipRequests { get; set; } = [];
}
