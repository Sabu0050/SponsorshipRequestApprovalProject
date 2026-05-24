using SponsorshipRequestApprovalProject.Application.Common.CQRS;
using SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.DTOs;

namespace SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.Commands.UpdateSponsorshipRequest;

public record UpdateSponsorshipRequestCommand(
    Guid SponsorshipRequestId,
    string Title,
    string Description,
    Guid SponsorshipTypeId,
    string SponsorName,
    decimal RequestedAmount,
    string CurrencyCode,
    DateOnly? EventDate,
    DateOnly? SponsorshipStartDate,
    DateOnly? SponsorshipEndDate,
    string CurrentUserId) : ICommand<CreateSponsorshipRequestResult>;
