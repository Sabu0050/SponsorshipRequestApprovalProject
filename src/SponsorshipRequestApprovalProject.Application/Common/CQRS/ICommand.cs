using MediatR;

namespace SponsorshipRequestApprovalProject.Application.Common.CQRS;

public interface ICommand<out TResponse> : IRequest<TResponse>
{
}
