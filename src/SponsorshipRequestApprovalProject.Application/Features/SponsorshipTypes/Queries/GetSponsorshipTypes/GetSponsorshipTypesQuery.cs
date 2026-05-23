using SponsorshipRequestApprovalProject.Application.Common.CQRS;
using SponsorshipRequestApprovalProject.Application.Features.SponsorshipTypes.DTOs;

namespace SponsorshipRequestApprovalProject.Application.Features.SponsorshipTypes.Queries.GetSponsorshipTypes;

public record GetSponsorshipTypesQuery(bool? IsActive) : IQuery<IReadOnlyCollection<SponsorshipTypeDto>>;
