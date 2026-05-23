using SponsorshipRequestApprovalProject.Application.Common.CQRS;
using SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.DTOs;

namespace SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.Commands.SubmitSponsorshipRequest;

public record SubmitSponsorshipRequestCommand(
    Guid SponsorshipRequestId,
    string PerformedById,
    string PerformedByName,
    string? AssignedManagerId,
    string? AssignedManagerName,
    string? Comments) : ICommand<SponsorshipRequestWorkflowResult>;
