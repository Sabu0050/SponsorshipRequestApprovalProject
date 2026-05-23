using SponsorshipRequestApprovalProject.Application.Features.Auth.DTOs;

namespace SponsorshipRequestApprovalProject.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<LoginResult?> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
}
