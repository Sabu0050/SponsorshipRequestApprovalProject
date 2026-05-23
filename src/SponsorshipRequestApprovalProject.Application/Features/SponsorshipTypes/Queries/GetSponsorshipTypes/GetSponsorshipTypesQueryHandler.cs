using Microsoft.EntityFrameworkCore;
using SponsorshipRequestApprovalProject.Application.Common.CQRS;
using SponsorshipRequestApprovalProject.Application.Common.Interfaces;
using SponsorshipRequestApprovalProject.Application.Features.SponsorshipTypes.DTOs;

namespace SponsorshipRequestApprovalProject.Application.Features.SponsorshipTypes.Queries.GetSponsorshipTypes;

public class GetSponsorshipTypesQueryHandler(IApplicationDbContext dbContext)
    : IQueryHandler<GetSponsorshipTypesQuery, IReadOnlyCollection<SponsorshipTypeDto>>
{
    public async Task<IReadOnlyCollection<SponsorshipTypeDto>> Handle(
        GetSponsorshipTypesQuery request,
        CancellationToken cancellationToken)
    {
        var query = dbContext.SponsorshipTypes
            .AsNoTracking()
            .AsQueryable();

        if (request.IsActive.HasValue)
        {
            query = query.Where(type => type.IsActive == request.IsActive.Value);
        }

        return await query
            .OrderBy(type => type.Name)
            .Select(type => new SponsorshipTypeDto(
                type.Id,
                type.Name,
                type.Description,
                type.IsActive,
                type.CreatedAt,
                type.UpdatedAt))
            .ToListAsync(cancellationToken);
    }
}
