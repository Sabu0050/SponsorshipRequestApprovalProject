using SponsorshipRequestApprovalProject.Api.Middleware;
using SponsorshipRequestApprovalProject.Api.Swagger;
using SponsorshipRequestApprovalProject.Application;
using SponsorshipRequestApprovalProject.Infrastructure;
using SponsorshipRequestApprovalProject.Infrastructure.Identity;

var builder = WebApplication.CreateBuilder(args);
const string CorsPolicyName = "ClientCors";
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>()
    ?? ["http://localhost:4200"];

builder.Services.AddControllers();
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

    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.ParameterLocation.Header,
        Description = "Enter a valid JWT bearer token."
    });

    options.OperationFilter<AuthorizeOperationFilter>();
});
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Sponsorship Request Approval API v1");
        options.DisplayRequestDuration();
    });
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors(CorsPolicyName);
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers().RequireCors(CorsPolicyName);

await app.Services.SeedIdentityAsync();

app.Run();
