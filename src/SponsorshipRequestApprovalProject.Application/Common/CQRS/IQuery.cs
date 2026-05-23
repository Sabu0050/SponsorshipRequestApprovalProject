using MediatR;

namespace SponsorshipRequestApprovalProject.Application.Common.CQRS;

public interface IQuery<out TResponse> : IRequest<TResponse>
{
}
