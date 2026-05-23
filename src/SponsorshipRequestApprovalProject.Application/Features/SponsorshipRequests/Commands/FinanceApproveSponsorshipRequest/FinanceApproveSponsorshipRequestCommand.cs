using SponsorshipRequestApprovalProject.Application.Common.CQRS;
using SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.DTOs;

namespace SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.Commands.FinanceApproveSponsorshipRequest;

public record FinanceApproveSponsorshipRequestCommand(
    Guid SponsorshipRequestId,
    string PerformedById,
    string PerformedByName,
    string? Comments) : ICommand<SponsorshipRequestWorkflowResult>;
