# Sponsorship Request Approval Workflow

Clean Architecture skeleton for a Sponsorship Request Approval Workflow system.

## Stack

- ASP.NET Core 10
- ASP.NET Core Identity
- CQRS with MediatR
- PostgreSQL via Entity Framework Core
- Angular
- Swagger/OpenAPI

## Structure

```text
src/
  SponsorshipRequestApprovalProject.Api/             HTTP API host and Swagger setup
  SponsorshipRequestApprovalProject.Application/     CQRS application layer
  SponsorshipRequestApprovalProject.Domain/          Domain model layer
  SponsorshipRequestApprovalProject.Infrastructure/  Persistence, Identity, external services
client/
  sponsorship-request-approval-project/               Angular client skeleton
tests/
  SponsorshipRequestApprovalProject.UnitTests/
  SponsorshipRequestApprovalProject.IntegrationTests/
```

No sponsorship workflow business logic has been implemented yet.
