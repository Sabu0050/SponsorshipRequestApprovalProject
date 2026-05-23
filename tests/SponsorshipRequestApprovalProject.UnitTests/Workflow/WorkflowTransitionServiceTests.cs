using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SponsorshipRequestApprovalProject.Application.Common.Exceptions;
using SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.Commands.Common;
using SponsorshipRequestApprovalProject.Domain.Entities;
using SponsorshipRequestApprovalProject.Domain.Enums;
using SponsorshipRequestApprovalProject.UnitTests.TestDoubles;

namespace SponsorshipRequestApprovalProject.UnitTests.Workflow;

public class WorkflowTransitionServiceTests
{
    [Fact]
    public async Task TransitionAsync_WithValidState_WritesHistoryAndUpdatesStatus()
    {
        var requestId = Guid.NewGuid();
        await using var dbContext = CreateDbContext();
        dbContext.SponsorshipRequests.Add(new SponsorshipRequest
        {
            Id = requestId,
            RequestNumber = "REQ-001",
            Title = "Draft request",
            Description = "Test",
            SponsorshipTypeId = Guid.NewGuid(),
            RequesterId = "requestor-1",
            RequesterName = "Requestor One",
            RequesterEmail = "requestor@example.com",
            SponsorName = "Sponsor",
            RequestedAmount = 1000m,
            CurrencyCode = "USD",
            Status = SponsorshipRequestStatus.Draft
        });
        await dbContext.SaveChangesAsync();

        var service = new WorkflowTransitionService(dbContext);

        var result = await service.TransitionAsync(
            requestId,
            SponsorshipRequestStatus.Draft,
            SponsorshipRequestStatus.PendingManagerApproval,
            WorkflowAction.Submitted,
            "user-1",
            "User One",
            "Submitted for approval",
            "manager-1",
            "Manager One",
            CancellationToken.None);

        result.SponsorshipRequestId.Should().Be(requestId);
        result.Status.Should().Be(SponsorshipRequestStatus.PendingManagerApproval);
        result.Action.Should().Be(WorkflowAction.Submitted);

        var updatedRequest = await dbContext.SponsorshipRequests.SingleAsync(r => r.Id == requestId);
        updatedRequest.Status.Should().Be(SponsorshipRequestStatus.PendingManagerApproval);
        updatedRequest.CurrentApproverId.Should().Be("manager-1");
        updatedRequest.CurrentApproverName.Should().Be("Manager One");
        updatedRequest.SubmittedAt.Should().NotBeNull();

        var history = await dbContext.WorkflowHistories.SingleAsync();
        history.Action.Should().Be(WorkflowAction.Submitted);
        history.FromStatus.Should().Be(SponsorshipRequestStatus.Draft);
        history.ToStatus.Should().Be(SponsorshipRequestStatus.PendingManagerApproval);
        history.PerformedById.Should().Be("user-1");
        history.PerformedByName.Should().Be("User One");
        history.Remarks.Should().Be("Submitted for approval");
        history.PerformedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task TransitionAsync_WithInvalidFromStatus_ThrowsBusinessRuleError()
    {
        var requestId = Guid.NewGuid();
        await using var dbContext = CreateDbContext();
        dbContext.SponsorshipRequests.Add(new SponsorshipRequest
        {
            Id = requestId,
            RequestNumber = "REQ-002",
            Title = "Pending request",
            Description = "Test",
            SponsorshipTypeId = Guid.NewGuid(),
            RequesterId = "requestor-1",
            RequesterName = "Requestor One",
            RequesterEmail = "requestor@example.com",
            SponsorName = "Sponsor",
            RequestedAmount = 1000m,
            CurrencyCode = "USD",
            Status = SponsorshipRequestStatus.PendingManagerApproval
        });
        await dbContext.SaveChangesAsync();

        var service = new WorkflowTransitionService(dbContext);

        var action = () => service.TransitionAsync(
            requestId,
            SponsorshipRequestStatus.Draft,
            SponsorshipRequestStatus.PendingFinanceReview,
            WorkflowAction.ManagerApproved,
            "manager-1",
            "Manager One",
            "Approve",
            "finance-1",
            "Finance One",
            CancellationToken.None);

        await action.Should()
            .ThrowAsync<WorkflowTransitionException>()
            .WithMessage("Cannot transition sponsorship request*");

        (await dbContext.WorkflowHistories.CountAsync()).Should().Be(0);
    }

    [Fact]
    public async Task TransitionAsync_WithTerminalStatus_ThrowsBusinessRuleError()
    {
        var requestId = Guid.NewGuid();
        await using var dbContext = CreateDbContext();
        dbContext.SponsorshipRequests.Add(new SponsorshipRequest
        {
            Id = requestId,
            RequestNumber = "REQ-003",
            Title = "Rejected request",
            Description = "Test",
            SponsorshipTypeId = Guid.NewGuid(),
            RequesterId = "requestor-1",
            RequesterName = "Requestor One",
            RequesterEmail = "requestor@example.com",
            SponsorName = "Sponsor",
            RequestedAmount = 1000m,
            CurrencyCode = "USD",
            Status = SponsorshipRequestStatus.Rejected
        });
        await dbContext.SaveChangesAsync();

        var service = new WorkflowTransitionService(dbContext);

        var action = () => service.TransitionAsync(
            requestId,
            SponsorshipRequestStatus.Rejected,
            SponsorshipRequestStatus.PendingManagerApproval,
            WorkflowAction.Submitted,
            "requestor-1",
            "Requestor One",
            "Retry",
            "manager-1",
            "Manager One",
            CancellationToken.None);

        await action.Should()
            .ThrowAsync<WorkflowTransitionException>()
            .WithMessage("Sponsorship request is terminal*");

        (await dbContext.WorkflowHistories.CountAsync()).Should().Be(0);
    }

    private static InMemoryApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<InMemoryApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new InMemoryApplicationDbContext(options);
    }
}
