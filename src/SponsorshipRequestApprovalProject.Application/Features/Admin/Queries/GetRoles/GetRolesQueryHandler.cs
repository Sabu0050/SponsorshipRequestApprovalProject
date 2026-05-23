using SponsorshipRequestApprovalProject.Application.Common.CQRS;
using SponsorshipRequestApprovalProject.Application.Common.Identity;
using SponsorshipRequestApprovalProject.Application.Features.Admin.DTOs;

namespace SponsorshipRequestApprovalProject.Application.Features.Admin.Queries.GetRoles;

public class GetRolesQueryHandler : IQueryHandler<GetRolesQuery, IReadOnlyCollection<RoleDto>>
{
    public Task<IReadOnlyCollection<RoleDto>> Handle(
        GetRolesQuery request,
        CancellationToken cancellationToken)
    {
        IReadOnlyCollection<RoleDto> roles = ApplicationRoles.All
            .Select(role => new RoleDto(role))
            .ToArray();

        return Task.FromResult(roles);
    }
}
