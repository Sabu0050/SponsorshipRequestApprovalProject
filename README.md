# Sponsorship Approval Workflow

Full-stack technical assessment implementation for a sponsorship request approval workflow.

## Project Links

- GitHub: [Sabu0050/SponsorshipRequestApprovalProject](https://github.com/Sabu0050/SponsorshipRequestApprovalProject)
- Backend API: [https://sponsordesk-api.onrender.com](https://sponsordesk-api.onrender.com)
- Frontend: [https://sponsordesk-approval.vercel.app/](https://sponsordesk-approval.vercel.app/)
- Swagger: [https://sponsordesk-api.onrender.com/swagger](https://sponsordesk-api.onrender.com/swagger)

## Stack

- Backend: ASP.NET Core Web API (.NET 10)
- Frontend: Angular (standalone components, Reactive Forms)
- Database: PostgreSQL (Neon in deployed environment)
- ORM: Entity Framework Core + Npgsql
- Auth: ASP.NET Core Identity + JWT + RBAC
- API docs: Swagger/OpenAPI
- Deployment: Render (API), Vercel (frontend), Neon (PostgreSQL)

## Features Implemented

- JWT login with role-aware landing routes
- Role and authority-based navigation + route guarding
- Requestor flow: create draft, submit, cancel, and track own requests
- Manager flow: pending approvals queue, approve/reject actions
- Finance flow: pending finance review queue, approve/reject actions
- Admin workspace: all requests, workflow history, sponsorship type management, role/user management
- Workflow history/audit timeline
- Status dashboards and count cards
- Dynamic login role list loaded from DB

## Workflow Statuses

- Draft
- PendingManagerApproval
- PendingFinanceReview
- Approved
- Rejected
- Cancelled

## Approval Rules

- Requestor can create/submit/cancel their own requests.
- Manager authority can process pending manager approvals.
- Finance authority can process pending finance reviews.
- Users cannot approve their own submitted requests.
- SystemAdmin can manage admin endpoints and view full workflow data.
- Every transition is logged to workflow history.

## Default Test Accounts

All seeded users use password: `Demo@12345`

| Role | Email |
|---|---|
| Requestor | `requestor.demo@company.com` |
| Manager | `manager.demo@company.com` |
| FinanceAdmin | `finance.demo@company.com` |
| SystemAdmin | `admin.demo@company.com` |

## Repository Structure

```text
src/
  SponsorshipRequestApprovalProject.Api/             API host, controllers, swagger
  SponsorshipRequestApprovalProject.Application/     CQRS handlers, DTOs, validation
  SponsorshipRequestApprovalProject.Domain/          Entities and enums
  SponsorshipRequestApprovalProject.Infrastructure/  Identity, EF Core, persistence
client/
  sponsorship-request-approval-project/              Angular frontend
tests/
  SponsorshipRequestApprovalProject.UnitTests/
  SponsorshipRequestApprovalProject.IntegrationTests/
```

## Local Setup

### Prerequisites

- .NET SDK 10
- Node.js 20+
- PostgreSQL (or Docker PostgreSQL)

### Backend

```bash
dotnet restore SponsorshipRequestApprovalProject.sln
dotnet ef database update --project src/SponsorshipRequestApprovalProject.Infrastructure --startup-project src/SponsorshipRequestApprovalProject.Api
dotnet run --project src/SponsorshipRequestApprovalProject.Api
```

Local Swagger:

- `https://localhost:7034/swagger` (or the port shown by launch profile)

### Frontend

```bash
cd client/sponsorship-request-approval-project
npm install
ng serve --open
```

## Environment Configuration

### Frontend

- Development: `src/environments/environment.ts`
- Production: `src/environments/environment.prod.ts`

Current production API base URL:

```ts
apiBaseUrl: 'https://sponsordesk-api.onrender.com/api'
```

### Backend

- Connection string key: `ConnectionStrings:DefaultConnection`
- JWT settings under: `Jwt`
- CORS list under: `Cors:AllowedOrigins`

Example deployed CORS origin:

- `https://sponsordesk-approval.vercel.app`

## Deployment Notes

- Backend is configured for Neon PostgreSQL and deployed on Render.
- Frontend is configured for Vercel with SPA rewrite support via `vercel.json`.
- Docker support is included at solution root (`Dockerfile`, `.dockerignore`).

## Useful Commands

```bash
dotnet test SponsorshipRequestApprovalProject.sln

cd client/sponsorship-request-approval-project
npm run build
```

## Notes / Tradeoffs

- Focus was on workflow correctness, RBAC behavior, and clear UI flows for assessment.
- Authority is managed at role level; user assignment is role-based.
- Login role cards are populated dynamically from DB for reviewer convenience.
