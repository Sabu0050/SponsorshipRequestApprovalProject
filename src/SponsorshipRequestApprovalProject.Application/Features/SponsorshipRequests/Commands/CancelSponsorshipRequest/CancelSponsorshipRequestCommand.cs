using SponsorshipRequestApprovalProject.Application.Common.CQRS;
using SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.DTOs;
using SponsorshipRequestApprovalProject.Domain.Enums;

namespace SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.Commands.CancelSponsorshipRequest;

public record CancelSponsorshipRequestCommand(
    Guid SponsorshipRequestId,
    SponsorshipRequestStatus ExpectedCurrentStatus,
    string PerformedById,
    string PerformedByName,
    string? Comments) : ICommand<SponsorshipRequestWorkflowResult>;
