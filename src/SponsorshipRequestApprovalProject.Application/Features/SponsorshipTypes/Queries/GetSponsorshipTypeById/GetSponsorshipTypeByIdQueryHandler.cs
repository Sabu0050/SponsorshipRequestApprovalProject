using Microsoft.EntityFrameworkCore;
using SponsorshipRequestApprovalProject.Application.Common.CQRS;
using SponsorshipRequestApprovalProject.Application.Common.Interfaces;
using SponsorshipRequestApprovalProject.Application.Features.SponsorshipTypes.DTOs;

namespace SponsorshipRequestApprovalProject.Application.Features.SponsorshipTypes.Queries.GetSponsorshipTypeById;

public class GetSponsorshipTypeByIdQueryHandler(IApplicationDbContext dbContext)
    : IQueryHandler<GetSponsorshipTypeByIdQuery, SponsorshipTypeDto?>
{
    public Task<SponsorshipTypeDto?> Handle(
        GetSponsorshipTypeByIdQuery request,
        CancellationToken cancellationToken)
    {
        return dbContext.SponsorshipTypes
            .AsNoTracking()
            .Where(type => type.Id == request.SponsorshipTypeId)
            .Select(type => new SponsorshipTypeDto(
                type.Id,
                type.Name,
                type.Description,
                type.IsActive,
                type.CreatedAt,
                type.UpdatedAt))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
