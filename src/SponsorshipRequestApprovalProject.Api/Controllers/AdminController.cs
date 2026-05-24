using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SponsorshipRequestApprovalProject.Api.Contracts.Admin;
using SponsorshipRequestApprovalProject.Application.Common.Identity;
using SponsorshipRequestApprovalProject.Infrastructure.Identity;

namespace SponsorshipRequestApprovalProject.Api.Controllers;

[ApiController]
[Authorize(Roles = ApplicationRoles.SystemAdmin)]
[Route("api/admin")]
public class AdminController(RoleManager<IdentityRole> roleManager) : ControllerBase
{
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;

    [HttpGet("roles")]
    public async Task<ActionResult<IReadOnlyCollection<object>>> GetRoles(
        CancellationToken cancellationToken)
    {
        var roles = await _roleManager.Roles
            .OrderBy(role => role.Name)
            .ToListAsync(cancellationToken);

        var result = new List<object>();
        foreach (var role in roles)
        {
            var claims = await _roleManager.GetClaimsAsync(role);
            result.Add(new
            {
                role.Name,
                CanRequestorAccess = HasRequestorAuthority(claims),
                CanApproveManagerStage = HasManagerAuthority(claims),
                CanApproveFinanceStage = HasFinanceAuthority(claims)
            });
        }

        return Ok(result);
    }

    [HttpPost("roles")]
    public async Task<ActionResult<object>> CreateRole(
        CreateRoleRequest request)
    {
        var name = request.Name?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(name))
        {
            return ValidationProblem(new ValidationProblemDetails(new Dictionary<string, string[]>
            {
                { nameof(request.Name), ["Role name is required."] }
            })
            {
                Title = "Please correct the highlighted fields and try again."
            });
        }

        if (await _roleManager.RoleExistsAsync(name))
        {
            return Conflict(new
            {
                title = "Role already exists.",
                detail = $"Role '{name}' already exists."
            });
        }

        var role = new IdentityRole(name);
        var result = await _roleManager.CreateAsync(role);
        if (!result.Succeeded)
        {
            return BadRequest(new
            {
                title = "Unable to create role.",
                errors = result.Errors.Select(error => error.Description).ToArray()
            });
        }

        await ApplyRoleAuthorityClaims(role, request.CanRequestorAccess, request.CanApproveManagerStage, request.CanApproveFinanceStage);

        return Created($"/api/admin/roles/{Uri.EscapeDataString(name)}", new { Name = name });
    }

    [HttpPut("roles/{name}")]
    public async Task<ActionResult<object>> UpdateRoleAuthorities(
        string name,
        UpdateRoleAuthoritiesRequest request)
    {
        var role = await _roleManager.FindByNameAsync(name);
        if (role is null)
        {
            return NotFound(new
            {
                title = "Role not found.",
                detail = $"Role '{name}' was not found."
            });
        }

        await ApplyRoleAuthorityClaims(role, request.CanRequestorAccess, request.CanApproveManagerStage, request.CanApproveFinanceStage);
        return Ok(new { role.Name });
    }

    [HttpGet("users")]
    public async Task<ActionResult<IReadOnlyCollection<object>>> GetUsers(
        [FromServices] UserManager<ApplicationUser> userManager,
        CancellationToken cancellationToken)
    {
        var users = await userManager.Users
            .OrderBy(user => user.Email)
            .ToListAsync(cancellationToken);

        var result = new List<object>();
        foreach (var user in users)
        {
            var roles = await userManager.GetRolesAsync(user);
            var roleName = roles.FirstOrDefault() ?? string.Empty;
            var roleEntity = string.IsNullOrWhiteSpace(roleName) ? null : await _roleManager.FindByNameAsync(roleName);
            var roleClaims = roleEntity is null ? [] : await _roleManager.GetClaimsAsync(roleEntity);
            result.Add(new
            {
                user.Id,
                user.Email,
                user.FirstName,
                user.LastName,
                user.Department,
                Role = roleName,
                CanRequestorAccess = HasRequestorAuthority(roleClaims),
                CanApproveManagerStage = HasManagerAuthority(roleClaims),
                CanApproveFinanceStage = HasFinanceAuthority(roleClaims)
            });
        }

        return Ok(result);
    }

    [HttpPost("users")]
    public async Task<ActionResult<object>> CreateUser(
        CreateAdminUserRequest request,
        [FromServices] UserManager<ApplicationUser> userManager)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return ValidationProblem(new ValidationProblemDetails(new Dictionary<string, string[]>
            {
                { nameof(request.Email), ["Email is required."] },
                { nameof(request.Password), ["Password is required."] }
            })
            {
                Title = "Please correct the highlighted fields and try again."
            });
        }

        if (!await _roleManager.RoleExistsAsync(request.Role))
        {
            return BadRequest(new
            {
                title = "Invalid role.",
                detail = $"Role '{request.Role}' does not exist."
            });
        }

        if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName))
        {
            return ValidationProblem(new ValidationProblemDetails(new Dictionary<string, string[]>
            {
                { nameof(request.FirstName), ["First name is required."] },
                { nameof(request.LastName), ["Last name is required."] }
            })
            {
                Title = "Please correct the highlighted fields and try again."
            });
        }

        var existing = await userManager.FindByEmailAsync(request.Email);
        if (existing is not null)
        {
            return Conflict(new
            {
                title = "User already exists.",
                detail = "A user with this email already exists."
            });
        }

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName?.Trim() ?? string.Empty,
            LastName = request.LastName?.Trim() ?? string.Empty,
            Department = request.Department?.Trim(),
            IsActive = true
        };

        var createResult = await userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
        {
            return BadRequest(new
            {
                title = "Unable to create user.",
                errors = createResult.Errors.Select(error => error.Description).ToArray()
            });
        }

        await userManager.AddToRoleAsync(user, request.Role);

        return Created($"/api/admin/users/{user.Id}", new { user.Id, user.Email, Role = request.Role });
    }

    [HttpPut("users/{id}")]
    public async Task<ActionResult<object>> UpdateUserAuthorities(
        string id,
        UpdateUserAuthoritiesRequest request,
        [FromServices] UserManager<ApplicationUser> userManager)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user is null)
        {
            return NotFound(new
            {
                title = "User not found.",
                detail = "The selected user could not be found."
            });
        }

        if (!await _roleManager.RoleExistsAsync(request.Role))
        {
            return BadRequest(new
            {
                title = "Invalid role.",
                detail = $"Role '{request.Role}' does not exist."
            });
        }

        if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName))
        {
            return ValidationProblem(new ValidationProblemDetails(new Dictionary<string, string[]>
            {
                { nameof(request.FirstName), ["First name is required."] },
                { nameof(request.LastName), ["Last name is required."] }
            })
            {
                Title = "Please correct the highlighted fields and try again."
            });
        }

        user.FirstName = request.FirstName?.Trim() ?? string.Empty;
        user.LastName = request.LastName?.Trim() ?? string.Empty;
        user.Department = request.Department?.Trim();
        var userUpdateResult = await userManager.UpdateAsync(user);
        if (!userUpdateResult.Succeeded)
        {
            return BadRequest(new
            {
                title = "Unable to update user.",
                errors = userUpdateResult.Errors.Select(error => error.Description).ToArray()
            });
        }

        var existingRoles = await userManager.GetRolesAsync(user);
        if (existingRoles.Count > 0)
        {
            await userManager.RemoveFromRolesAsync(user, existingRoles);
        }

        await userManager.AddToRoleAsync(user, request.Role);

        return Ok(new { user.Id, user.Email, Role = request.Role });
    }

    private async Task ApplyRoleAuthorityClaims(
        IdentityRole role,
        bool canRequestorAccess,
        bool canApproveManagerStage,
        bool canApproveFinanceStage)
    {
        var existingClaims = await _roleManager.GetClaimsAsync(role);
        var authorityClaims = existingClaims.Where(c =>
            c.Type == ApprovalStages.ClaimType
            || c.Type == ApprovalStages.AccessClaimType).ToArray();
        if (authorityClaims.Length > 0)
        {
            foreach (var claim in authorityClaims)
            {
                var removeResult = await _roleManager.RemoveClaimAsync(role, claim);
                if (!removeResult.Succeeded)
                {
                    throw new InvalidOperationException(string.Join("; ", removeResult.Errors.Select(error => error.Description)));
                }
            }
        }

        if (canRequestorAccess)
        {
            var addRequestorResult = await _roleManager.AddClaimAsync(role, new System.Security.Claims.Claim(ApprovalStages.AccessClaimType, ApprovalStages.Requestor));
            if (!addRequestorResult.Succeeded)
            {
                throw new InvalidOperationException(string.Join("; ", addRequestorResult.Errors.Select(error => error.Description)));
            }
        }

        if (canApproveManagerStage)
        {
            var addManagerResult = await _roleManager.AddClaimAsync(role, new System.Security.Claims.Claim(ApprovalStages.ClaimType, ApprovalStages.Manager));
            if (!addManagerResult.Succeeded)
            {
                throw new InvalidOperationException(string.Join("; ", addManagerResult.Errors.Select(error => error.Description)));
            }
        }

        if (canApproveFinanceStage)
        {
            var addFinanceResult = await _roleManager.AddClaimAsync(role, new System.Security.Claims.Claim(ApprovalStages.ClaimType, ApprovalStages.Finance));
            if (!addFinanceResult.Succeeded)
            {
                throw new InvalidOperationException(string.Join("; ", addFinanceResult.Errors.Select(error => error.Description)));
            }
        }
    }

    private static bool HasRequestorAuthority(IEnumerable<System.Security.Claims.Claim> claims)
    {
        return claims.Any(claim =>
        {
            var type = claim.Type?.Trim().ToLowerInvariant() ?? string.Empty;
            var value = claim.Value?.Trim().ToLowerInvariant() ?? string.Empty;
            var isAuthorityType = type.Contains("access") || type.Contains("approval") || type.Contains("claim");
            return isAuthorityType && (value == "requestoraccess" || value == "requestor" || value.Contains("requestor"));
        });
    }

    private static bool HasManagerAuthority(IEnumerable<System.Security.Claims.Claim> claims)
    {
        return claims.Any(claim =>
        {
            var type = claim.Type?.Trim().ToLowerInvariant() ?? string.Empty;
            var value = claim.Value?.Trim().ToLowerInvariant() ?? string.Empty;
            var isAuthorityType = type.Contains("approval") || type.Contains("access") || type.Contains("claim");
            return isAuthorityType && (value == "managerapproval" || value == "manager" || value.Contains("manager"));
        });
    }

    private static bool HasFinanceAuthority(IEnumerable<System.Security.Claims.Claim> claims)
    {
        return claims.Any(claim =>
        {
            var type = claim.Type?.Trim().ToLowerInvariant() ?? string.Empty;
            var value = claim.Value?.Trim().ToLowerInvariant() ?? string.Empty;
            var isAuthorityType = type.Contains("approval") || type.Contains("access") || type.Contains("claim");
            return isAuthorityType && (value == "financeapproval" || value == "finance" || value.Contains("finance"));
        });
    }
}
