using SponsorshipRequestApprovalProject.Application.Common.CQRS;
using SponsorshipRequestApprovalProject.Application.Common.Interfaces;
using SponsorshipRequestApprovalProject.Application.Features.Auth.DTOs;

namespace SponsorshipRequestApprovalProject.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler(IIdentityService identityService)
    : ICommandHandler<LoginCommand, LoginResult?>
{
    public Task<LoginResult?> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        return identityService.LoginAsync(request.Email, request.Password, cancellationToken);
    }
}
