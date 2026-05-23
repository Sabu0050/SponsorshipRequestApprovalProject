using SponsorshipRequestApprovalProject.Application.Common.CQRS;
using SponsorshipRequestApprovalProject.Application.Features.Admin.DTOs;

namespace SponsorshipRequestApprovalProject.Application.Features.Admin.Queries.GetRoles;

public record GetRolesQuery : IQuery<IReadOnlyCollection<RoleDto>>;
