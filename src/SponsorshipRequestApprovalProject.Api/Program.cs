using SponsorshipRequestApprovalProject.Api.Middleware;
using SponsorshipRequestApprovalProject.Api.Swagger;
using SponsorshipRequestApprovalProject.Application;
using SponsorshipRequestApprovalProject.Infrastructure;
using SponsorshipRequestApprovalProject.Infrastructure.Identity;
using SponsorshipRequestApprovalProject.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
const string CorsPolicyName = "ClientCors";
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>()
    ?? ["http://localhost:4200"];

builder.Services.AddControllers();
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(entry => entry.Value?.Errors.Count > 0)
            .ToDictionary(
                entry => entry.Key,
                entry => entry.Value!.Errors
                    .Select(error => string.IsNullOrWhiteSpace(error.ErrorMessage) ? "Invalid value." : error.ErrorMessage)
                    .ToArray());

        return new BadRequestObjectResult(new
        {
            title = "Please correct the highlighted fields and try again.",
            status = StatusCodes.Status400BadRequest,
            traceId = context.HttpContext.TraceIdentifier,
            errors
        });
    };
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicyName, policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
    {
        Title = "Sponsorship Request Approval API",
        Version = "v1"
    });

    // Stable bearer setup for Swagger UI authorize flow
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.ParameterLocation.Header,
        Description = "Enter JWT access token only. Do not include the word Bearer."
    });

    options.AddSecurityRequirement(_ => new Microsoft.OpenApi.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.OpenApiSecuritySchemeReference("Bearer", null, null),
            []
        }
    });

    options.OperationFilter<AuthorizeOperationFilter>();
});
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();


    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Sponsorship Request Approval API v1");
        options.DisplayRequestDuration();
    });


app.UseHttpsRedirection();
app.UseRouting();
app.UseCors(CorsPolicyName);
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
app.UseStatusCodePages(async statusCodeContext =>
{
    var response = statusCodeContext.HttpContext.Response;
    if (response.HasStarted)
    {
        return;
    }

    if (response.ContentLength is > 0 || !string.IsNullOrWhiteSpace(response.ContentType))
    {
        return;
    }

    var message = response.StatusCode switch
    {
        StatusCodes.Status401Unauthorized => "Authentication is required to access this resource.",
        StatusCodes.Status403Forbidden => "You do not have permission to perform this action.",
        StatusCodes.Status404NotFound => "The requested resource could not be found.",
        _ => "Request could not be processed."
    };

    response.ContentType = "application/json";
    await response.WriteAsJsonAsync(new
    {
        title = message,
        status = response.StatusCode,
        traceId = statusCodeContext.HttpContext.TraceIdentifier
    });
});
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers().RequireCors(CorsPolicyName);

/*await app.Services.SeedIdentityAsync();
await app.Services.SeedSponsorshipTypesAsync();
*/
app.Run();
