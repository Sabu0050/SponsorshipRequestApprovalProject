namespace SponsorshipRequestApprovalProject.Api.Contracts.SponsorshipRequests;

public record UpdateSponsorshipRequestRequest(
    string Title,
    string Description,
    Guid SponsorshipTypeId,
    string SponsorName,
    decimal RequestedAmount,
    string CurrencyCode,
    DateOnly? EventDate,
    DateOnly? SponsorshipStartDate,
    DateOnly? SponsorshipEndDate);
