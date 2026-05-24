using SponsorshipRequestApprovalProject.Application.Common.CQRS;
using SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.DTOs;

namespace SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.Commands.CreateSponsorshipRequest;

public record CreateSponsorshipRequestCommand(
    string Title,
    string Description,
    Guid SponsorshipTypeId,
    string SponsorName,
    decimal RequestedAmount,
    string CurrencyCode,
    DateOnly? EventDate,
    DateOnly? SponsorshipStartDate,
    DateOnly? SponsorshipEndDate,
    string RequesterId,
    string RequesterName,
    string RequesterEmail) : ICommand<CreateSponsorshipRequestResult>;
