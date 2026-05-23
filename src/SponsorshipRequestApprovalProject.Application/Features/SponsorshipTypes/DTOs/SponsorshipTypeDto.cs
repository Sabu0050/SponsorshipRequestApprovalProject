namespace SponsorshipRequestApprovalProject.Application.Features.SponsorshipTypes.DTOs;

public record SponsorshipTypeDto(
    Guid Id,
    string Name,
    string? Description,
    bool IsActive,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);
