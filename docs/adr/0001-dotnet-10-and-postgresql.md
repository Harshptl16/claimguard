# ADR 0001: ASP.NET Core on .NET 10 with PostgreSQL

**Status:** Accepted · 2026-11-23

## Context
The project mirrors an enterprise Corporate IT stack (Azure, Entra ID). It needs a mainstream, long-term-supported framework with built-in authentication, rate limiting and OpenAPI support.

## Decision
- **.NET 10 (LTS)** with ASP.NET Core. .NET 8 reached end of support in November 2026, so a new project shouldn't start on it.
- **PostgreSQL** in Docker locally, accessed through EF Core with migrations. All queries are parameterized through the ORM.
- **Chiseled runtime image**: no shell or package manager, non-root by default.

## Consequences
- In-box rate limiter, ProblemDetails and health checks mean fewer third-party packages to keep patched.
- Chiseled images can't run `curl` for container health checks, so liveness is probed externally (`/health`).
