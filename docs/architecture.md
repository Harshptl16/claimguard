# Architecture

> Filled in during Sprint 1 and finished in Sprint 5.

Request path: client → authentication (JWT) → controller (routing + role policy) → service (business rules + ownership check) → EF Core → PostgreSQL.

The AI module redacts PII before calling the LLM and validates its reply against a JSON schema. The CI pipeline scans every change before it deploys.

TODO (Sprint 5): add the diagram from the project plan and a data-flow diagram with trust boundaries.
