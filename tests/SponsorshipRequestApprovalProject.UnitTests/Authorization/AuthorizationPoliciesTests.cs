using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SponsorshipRequestApprovalProject.Application.Common.Identity;
using SponsorshipRequestApprovalProject.Infrastructure;

namespace SponsorshipRequestApprovalProject.UnitTests.Authorization;

public class AuthorizationPoliciesTests
{
    [Fact]
    public async Task ManagerPolicy_AllowsManagerRole_AndRejectsRequestorRole()
    {
        var serviceProvider = BuildServiceProvider();
        var authorizationService = serviceProvider.GetRequiredService<IAuthorizationService>();

        var manager = CreateUser("manager-1", ApplicationRoles.Manager);
        var requestor = CreateUser("requestor-1", ApplicationRoles.Requestor);

        var managerResult = await authorizationService.AuthorizeAsync(manager, resource: null, ApplicationRoles.Manager);
        var requestorResult = await authorizationService.AuthorizeAsync(requestor, resource: null, ApplicationRoles.Manager);

        managerResult.Succeeded.Should().BeTrue();
        requestorResult.Succeeded.Should().BeFalse();
    }

    [Fact]
    public async Task SystemAdminPolicy_AllowsOnlySystemAdminRole()
    {
        var serviceProvider = BuildServiceProvider();
        var authorizationService = serviceProvider.GetRequiredService<IAuthorizationService>();

        var admin = CreateUser("admin-1", ApplicationRoles.SystemAdmin);
        var finance = CreateUser("finance-1", ApplicationRoles.FinanceAdmin);

        var adminResult = await authorizationService.AuthorizeAsync(admin, resource: null, ApplicationRoles.SystemAdmin);
        var financeResult = await authorizationService.AuthorizeAsync(finance, resource: null, ApplicationRoles.SystemAdmin);

        adminResult.Succeeded.Should().BeTrue();
        financeResult.Succeeded.Should().BeFalse();
    }

    private static ServiceProvider BuildServiceProvider()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Port=5432;Database=tests;Username=postgres;Password=postgres",
                ["Jwt:Issuer"] = "tests",
                ["Jwt:Audience"] = "tests",
                ["Jwt:SigningKey"] = "unit-tests-signing-key-which-is-long-enough",
                ["Jwt:ExpirationMinutes"] = "60"
            })
            .Build();

        var services = new ServiceCollection();
        services.AddInfrastructure(configuration);

        return services.BuildServiceProvider();
    }

    private static ClaimsPrincipal CreateUser(string userId, string role)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Role, role)
        };

        return new ClaimsPrincipal(new ClaimsIdentity(claims, "test"));
    }
}
