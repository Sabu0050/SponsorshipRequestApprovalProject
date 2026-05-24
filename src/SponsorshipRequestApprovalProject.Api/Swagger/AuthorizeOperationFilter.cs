using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SponsorshipRequestApprovalProject.Api.Swagger;

public class AuthorizeOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var endpointMetadata = context.ApiDescription.ActionDescriptor.EndpointMetadata;

        var hasAllowAnonymousOnEndpoint = endpointMetadata.OfType<IAllowAnonymous>().Any();
        var hasAllowAnonymousOnMethod = context.MethodInfo
            .GetCustomAttributes(true)
            .OfType<IAllowAnonymous>()
            .Any();

        if (hasAllowAnonymousOnEndpoint || hasAllowAnonymousOnMethod)
        {
            return;
        }

        var hasAuthorizeOnEndpoint = endpointMetadata.OfType<IAuthorizeData>().Any();
        var hasAuthorizeOnMethod = context.MethodInfo
            .GetCustomAttributes(true)
            .OfType<IAuthorizeData>()
            .Any();
        var hasAuthorizeOnController = context.MethodInfo.DeclaringType?
            .GetCustomAttributes(true)
            .OfType<IAuthorizeData>()
            .Any() == true;

        if (!hasAuthorizeOnEndpoint && !hasAuthorizeOnMethod && !hasAuthorizeOnController)
        {
            return;
        }

        operation.Security ??= [];
        operation.Security.Add(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecuritySchemeReference("Bearer", null, null),
                []
            }
        });
    }
}
