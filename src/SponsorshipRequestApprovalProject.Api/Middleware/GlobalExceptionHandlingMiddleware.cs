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
                title = "Please correct the highlighted fields and try again.",
                status = StatusCodes.Status400BadRequest,
                traceId = context.TraceIdentifier,
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
                title = "This action could not be completed due to workflow rules.",
                status = StatusCodes.Status422UnprocessableEntity,
                detail = exception.Message,
                traceId = context.TraceIdentifier
            });
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unhandled exception occurred while processing request.");

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new
            {
                title = "Something went wrong while processing your request.",
                detail = "Please try again in a moment. If the issue persists, contact support.",
                status = StatusCodes.Status500InternalServerError,
                traceId = context.TraceIdentifier
            });
        }
    }
}
