using System.ComponentModel.DataAnnotations;

namespace SponsorshipRequestApprovalProject.Api.Contracts.SponsorshipRequests;

public record CreateSponsorshipRequestRequest(
    [param: Required, MinLength(1)] string Title,
    [param: Required, MinLength(1)] string Description,
    Guid SponsorshipTypeId,
    [param: Required, MinLength(1)] string SponsorName,
    [param: Range(typeof(decimal), "0.01", "999999999999")] decimal RequestedAmount,
    [param: Required, MinLength(1)] string CurrencyCode,
    DateOnly? EventDate,
    DateOnly? SponsorshipStartDate,
    DateOnly? SponsorshipEndDate);
