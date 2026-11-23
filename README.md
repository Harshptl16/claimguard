# ClaimGuard

A secure insurance-claims REST API with a guardrailed Gen AI assistant, built on ASP.NET Core (.NET 10) and PostgreSQL.
Every security control is mapped to the [OWASP API Security Top 10 (2023)](https://owasp.org/API-Security/editions/2023/en/0x11-t10/) and backed by an automated test or scan.

> **Status:** Sprint 0 (scaffold). All data is synthetic. This is a personal learning project, not affiliated with any employer.

## Run it

**Prerequisites:** .NET 10 SDK, Docker Desktop.

```bash
cp .env.example .env          # then set your own POSTGRES_PASSWORD
docker compose up --build
curl http://localhost:8080/health      # -> Healthy
curl http://localhost:8080/api/v1      # -> API info
```

Without Docker:

```bash
dotnet run --project src/ClaimGuard.Api
# OpenAPI document (Development only): http://localhost:5262/openapi/v1.json
```

## Test it

```bash
dotnet test
```

Integration tests boot the real pipeline in memory and check the security baseline: headers, ProblemDetails errors without internals, and per-caller rate limiting.

## Security features

| Control | OWASP | Status | Proven by |
|---|---|---|---|
| Security headers, no server banner | API8 | ✅ Sprint 0 | `BaselineSecurityTests`, `SecurityHeadersMiddlewareTests` |
| ProblemDetails errors, no stack traces | API8 | ✅ Sprint 0 | `BaselineSecurityTests` |
| Per-caller rate limiting (100/min) | API4 | ✅ Sprint 0 | `BaselineSecurityTests` |
| Non-root, chiseled container image | API8 | ✅ Sprint 0 | Trivy scan in CI |
| Secret, SAST and image scanning in CI | — | ✅ Sprint 0 | `.github/workflows/ci.yml` |
| Object-level authorization (BOLA) | API1 | ⏳ Sprint 2 | |
| JWT authentication | API2 | ⏳ Sprint 2 | |
| DTOs only, no mass assignment | API3 | ⏳ Sprint 1 | |
| Role policies, deny by default | API5 | ⏳ Sprint 2 | |
| LLM output treated as untrusted | API10 | ⏳ Sprint 4 | |

The full threat model is in [`docs/threat-model.md`](docs/threat-model.md).

## Project layout

```
src/ClaimGuard.Api/
  Controllers/   thin: routing + auth attributes only
  Services/      business rules + ownership checks
  Data/          DbContext, entities, migrations
  Dtos/          request/response models (never entities)
  Security/      headers, rate limits, JWT, policies
  Ai/            redactor, prompt, schema validation
tests/           unit + integration (Testcontainers, negative security tests)
etl/             nightly reporting job
docs/            architecture, threat model, SSDF mapping, vuln register, ADRs
```

## Docs

- [Architecture](docs/architecture.md)
- [Threat model](docs/threat-model.md)
- [NIST SSDF mapping](docs/ssdf-mapping.md)
- [Vulnerability register](docs/vuln-register.md)
- [Architecture decision records](docs/adr/)
- [User stories](docs/user-stories.md)
- [Security policy](SECURITY.md)
