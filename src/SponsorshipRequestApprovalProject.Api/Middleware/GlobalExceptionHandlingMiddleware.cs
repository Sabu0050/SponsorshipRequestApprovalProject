using FluentValidation;
using SponsorshipRequestApprovalProject.Application.Common.Exceptions;

namespace SponsorshipRequestApprovalProject.Api.Middleware;

public class GlobalExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<GlobalExceptionHandlingMiddleware> logger)
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
        catch (BusinessRuleException exception)
        {
            context.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
            await context.Response.WriteAsJsonAsync(new
            {
                title = "Business rule violation.",
                status = StatusCodes.Status422UnprocessableEntity,
                detail = exception.Message
            });
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unhandled exception occurred while processing request.");

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new
            {
                title = "An unexpected error occurred.",
                status = StatusCodes.Status500InternalServerError
            });
        }
    }
}
