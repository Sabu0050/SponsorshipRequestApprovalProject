using Microsoft.EntityFrameworkCore;
using SponsorshipRequestApprovalProject.Application.Common.CQRS;
using SponsorshipRequestApprovalProject.Application.Common.Interfaces;
using SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.DTOs;
using SponsorshipRequestApprovalProject.Domain.Enums;

namespace SponsorshipRequestApprovalProject.Application.Features.SponsorshipRequests.Commands.UpdateSponsorshipRequest;

public class UpdateSponsorshipRequestCommandHandler(IApplicationDbContext dbContext)
    : ICommandHandler<UpdateSponsorshipRequestCommand, CreateSponsorshipRequestResult>
{
    public async Task<CreateSponsorshipRequestResult> Handle(
        UpdateSponsorshipRequestCommand request,
        CancellationToken cancellationToken)
    {
        var entity = await dbContext.SponsorshipRequests
            .FirstOrDefaultAsync(item => item.Id == request.SponsorshipRequestId, cancellationToken);

        if (entity is null)
        {
            throw new InvalidOperationException("Sponsorship request was not found.");
        }

        if (!string.Equals(entity.RequesterId, request.CurrentUserId, StringComparison.Ordinal))
        {
            throw new InvalidOperationException("You are not allowed to update this sponsorship request.");
        }

        if (entity.Status != SponsorshipRequestStatus.Draft)
        {
            throw new InvalidOperationException("Only draft sponsorship requests can be updated.");
        }

        var sponsorshipTypeExists = await dbContext.SponsorshipTypes
            .AsNoTracking()
            .AnyAsync(type => type.Id == request.SponsorshipTypeId && type.IsActive, cancellationToken);

        if (!sponsorshipTypeExists)
        {
            throw new InvalidOperationException("Selected sponsorship type is invalid or inactive.");
        }

        entity.Title = request.Title.Trim();
        entity.Description = request.Description.Trim();
        entity.SponsorshipTypeId = request.SponsorshipTypeId;
        entity.SponsorName = request.SponsorName.Trim();
        entity.RequestedAmount = request.RequestedAmount;
        entity.CurrencyCode = request.CurrencyCode.Trim().ToUpperInvariant();
        entity.EventDate = request.EventDate;
        entity.SponsorshipStartDate = request.SponsorshipStartDate;
        entity.SponsorshipEndDate = request.SponsorshipEndDate;
        entity.UpdatedAt = DateTimeOffset.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateSponsorshipRequestResult(
            entity.Id,
            entity.RequestNumber,
            entity.Status);
    }
}
