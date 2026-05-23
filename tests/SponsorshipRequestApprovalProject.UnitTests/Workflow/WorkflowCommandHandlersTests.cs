using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.Commands.Common;
using SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.Commands.FinanceApproveSponsorshipRequest;
using SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.Commands.ManagerApproveSponsorshipRequest;
using SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.Commands.SubmitSponsorshipRequest;
using SponsorshipRequestApprovalProject.Domain.Entities;
using SponsorshipRequestApprovalProject.Domain.Enums;
using SponsorshipRequestApprovalProject.UnitTests.TestDoubles;

namespace SponsorshipRequestApprovalProject.UnitTests.Workflow;

public class WorkflowCommandHandlersTests
{
    [Fact]
    public async Task SubmitHandler_WithDraftRequest_TransitionsAndLogsHistory()
    {
        var requestId = Guid.NewGuid();
        await using var dbContext = CreateDbContext();
        dbContext.SponsorshipRequests.Add(CreateRequest(requestId, SponsorshipRequestStatus.Draft));
        await dbContext.SaveChangesAsync();

        var service = new WorkflowTransitionService(dbContext);
        var handler = new SubmitSponsorshipRequestCommandHandler(service);

        var result = await handler.Handle(
            new SubmitSponsorshipRequestCommand(
                requestId,
                "requestor-1",
                "Requestor One",
                "manager-1",
                "Manager One",
                "Submit"),
            CancellationToken.None);

        result.Status.Should().Be(SponsorshipRequestStatus.PendingManagerApproval);
        result.Action.Should().Be(WorkflowAction.Submitted);

        var history = await dbContext.WorkflowHistories.SingleAsync();
        history.Action.Should().Be(WorkflowAction.Submitted);
        history.FromStatus.Should().Be(SponsorshipRequestStatus.Draft);
        history.ToStatus.Should().Be(SponsorshipRequestStatus.PendingManagerApproval);
    }

    [Fact]
    public async Task ManagerApproveHandler_WithPendingManagerRequest_TransitionsToFinanceReview()
    {
        var requestId = Guid.NewGuid();
        await using var dbContext = CreateDbContext();
        dbContext.SponsorshipRequests.Add(CreateRequest(requestId, SponsorshipRequestStatus.PendingManagerApproval));
        await dbContext.SaveChangesAsync();

        var service = new WorkflowTransitionService(dbContext);
        var handler = new ManagerApproveSponsorshipRequestCommandHandler(service);

        var result = await handler.Handle(
            new ManagerApproveSponsorshipRequestCommand(
                requestId,
                "manager-1",
                "Manager One",
                "finance-1",
                "Finance One",
                "Approved by manager"),
            CancellationToken.None);

        result.Status.Should().Be(SponsorshipRequestStatus.PendingFinanceReview);
        result.Action.Should().Be(WorkflowAction.ManagerApproved);

        var updated = await dbContext.SponsorshipRequests.SingleAsync(r => r.Id == requestId);
        updated.CurrentApproverId.Should().Be("finance-1");
        updated.CurrentApproverName.Should().Be("Finance One");
    }

    [Fact]
    public async Task FinanceApproveHandler_WithPendingFinanceRequest_TransitionsToApprovedAndLogs()
    {
        var requestId = Guid.NewGuid();
        await using var dbContext = CreateDbContext();
        dbContext.SponsorshipRequests.Add(CreateRequest(requestId, SponsorshipRequestStatus.PendingFinanceReview));
        await dbContext.SaveChangesAsync();

        var service = new WorkflowTransitionService(dbContext);
        var handler = new FinanceApproveSponsorshipRequestCommandHandler(service);

        var result = await handler.Handle(
            new FinanceApproveSponsorshipRequestCommand(
                requestId,
                "finance-1",
                "Finance One",
                "Approved"),
            CancellationToken.None);

        result.Status.Should().Be(SponsorshipRequestStatus.Approved);
        result.Action.Should().Be(WorkflowAction.FinanceApproved);

        var request = await dbContext.SponsorshipRequests.SingleAsync(r => r.Id == requestId);
        request.FinalDecisionById.Should().Be("finance-1");
        request.DecisionNotes.Should().Be("Approved");

        var history = await dbContext.WorkflowHistories.SingleAsync();
        history.Action.Should().Be(WorkflowAction.FinanceApproved);
        history.Remarks.Should().Be("Approved");
    }

    private static SponsorshipRequest CreateRequest(Guid id, SponsorshipRequestStatus status)
    {
        return new SponsorshipRequest
        {
            Id = id,
            RequestNumber = $"REQ-{id:N}"[..12],
            Title = "Request",
            Description = "Description",
            SponsorshipTypeId = Guid.NewGuid(),
            RequesterId = "requestor-1",
            RequesterName = "Requestor One",
            RequesterEmail = "requestor@example.com",
            SponsorName = "Sponsor",
            RequestedAmount = 1000m,
            CurrencyCode = "USD",
            Status = status
        };
    }

    private static InMemoryApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<InMemoryApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new InMemoryApplicationDbContext(options);
    }
}
