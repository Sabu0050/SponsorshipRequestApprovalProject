using Microsoft.EntityFrameworkCore;
using SponsorshipRequestApprovalProject.Application.Common.CQRS;
using SponsorshipRequestApprovalProject.Application.Common.Interfaces;
using SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.DTOs;
using SponsorshipRequestApprovalProject.Domain.Entities;
using SponsorshipRequestApprovalProject.Domain.Enums;

namespace SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.Commands.CreateSponsorshipRequest;

public class CreateSponsorshipRequestCommandHandler(IApplicationDbContext dbContext)
    : ICommandHandler<CreateSponsorshipRequestCommand, CreateSponsorshipRequestResult>
{
    public async Task<CreateSponsorshipRequestResult> Handle(
        CreateSponsorshipRequestCommand request,
        CancellationToken cancellationToken)
    {
        var sponsorshipTypeExists = await dbContext.SponsorshipTypes
            .AsNoTracking()
            .AnyAsync(type => type.Id == request.SponsorshipTypeId && type.IsActive, cancellationToken);

        if (!sponsorshipTypeExists)
        {
            throw new InvalidOperationException("Selected sponsorship type is invalid or inactive.");
        }

        var utcNow = DateTimeOffset.UtcNow;
        var requestNumber = $"SR-{utcNow:yyyyMMddHHmmss}-{Guid.NewGuid():N}"[..32];

        var entity = new SponsorshipRequest
        {
            Id = Guid.NewGuid(),
            RequestNumber = requestNumber,
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            SponsorshipTypeId = request.SponsorshipTypeId,
            RequesterId = request.RequesterId,
            RequesterName = request.RequesterName.Trim(),
            RequesterEmail = request.RequesterEmail.Trim(),
            SponsorName = request.SponsorName.Trim(),
            RequestedAmount = request.RequestedAmount,
            CurrencyCode = request.CurrencyCode.Trim().ToUpperInvariant(),
            EventDate = request.EventDate,
            SponsorshipStartDate = request.SponsorshipStartDate,
            SponsorshipEndDate = request.SponsorshipEndDate,
            Status = SponsorshipRequestStatus.Draft,
            CreatedAt = utcNow
        };

        dbContext.SponsorshipRequests.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateSponsorshipRequestResult(
            entity.Id,
            entity.RequestNumber,
            entity.Status);
    }
}
