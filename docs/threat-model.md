# Threat model (STRIDE)

> Written in Sprint 5. Each threat links to a control in the README's security table and the test that proves it.

## Scope and assets
- Assets: claim records, policyholder PII, JWT signing key, LLM API token, audit log
- Trust boundaries: internet → API, API → PostgreSQL, API → LLM provider, CI → Azure

## Threats

| ID | Component | STRIDE | Threat | Mitigation | Test |
|---|---|---|---|---|---|
| T1 | Claims API | Information disclosure | Customer reads another customer's claim by changing the ID (BOLA) | Ownership check in the service layer; 404 response | `ClaimAuthorizationTests` (Sprint 2) |
| T2 | | | | | |
