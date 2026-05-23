using SponsorshipRequestApprovalProject.Application.Common.CQRS;
using SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.DTOs;

namespace SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.Commands.ManagerApproveSponsorshipRequest;

public record ManagerApproveSponsorshipRequestCommand(
    Guid SponsorshipRequestId,
    string PerformedById,
    string PerformedByName,
    string? AssignedFinanceReviewerId,
    string? AssignedFinanceReviewerName,
    string? Comments) : ICommand<SponsorshipRequestWorkflowResult>;
