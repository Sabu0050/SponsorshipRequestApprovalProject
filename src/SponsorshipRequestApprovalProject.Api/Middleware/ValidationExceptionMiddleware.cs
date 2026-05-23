using FluentValidation;
using SponsorshipRequestApprovalProject.Application.Common.Exceptions;

namespace SponsorshipRequestApprovalProject.Api.Middleware;

public class ValidationExceptionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException exception)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new
            {
                title = "Validation failed.",
                status = StatusCodes.Status400BadRequest,
                errors = exception.Errors
                    .GroupBy(error => error.PropertyName)
                    .ToDictionary(
                        group => group.Key,
                        group => group.Select(error => error.ErrorMessage).ToArray())
            });
        }
        catch (WorkflowTransitionException exception)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new
            {
                title = "Invalid workflow transition.",
                status = StatusCodes.Status400BadRequest,
                detail = exception.Message
            });
        }
    }
}
