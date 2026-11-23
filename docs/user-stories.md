# ClaimGuard user stories (initial backlog)

Each story becomes a GitHub issue on the Projects board. `scripts/create-issues.sh` creates them for you.
Security stories reference the control IDs in section 5 of the project plan (OWASP API Top 10, 2023).

| # | Sprint | Story | Acceptance criteria |
|---|--------|-------|---------------------|
| 1 | 1 | As a **customer**, I can file a claim against my own policy so that I can get reimbursed. | `POST /api/v1/claims` returns 201 with the claim in `Submitted` status. Filing against someone else's policy returns 404 (API1). Client-sent `status` is ignored (API3). |
| 2 | 1 | As a **customer**, I can view my claim so that I can track its progress. | `GET /api/v1/claims/{id}` returns my claim. Another customer's claim returns 404, never 403, so IDs can't be probed (API1). |
| 3 | 1 | As an **adjuster**, I can see a paginated queue of submitted claims so that I can work through them in order. | Filter by status. Page size is capped at 50; larger values are clamped (API4). Customers get 403 (API5). |
| 4 | 1 | As an **adjuster**, I can move a claim through Submitted → In review → Approved/Denied so that decisions are recorded. | Invalid transitions (e.g. Submitted → Approved) return 409. Every change writes an audit row with who, what and when. |
| 5 | 1 | As an **owner or adjuster**, I can add a note to a claim so that context stays with the claim. | Notes are limited to 2,000 characters (API4). HTML is stored as plain text and never rendered. |
| 6 | 2 | As a **user**, I can sign in and get a short-lived token so that my session can't be reused for long if stolen. | JWT expires in 15 minutes. Expired, tampered and `alg: none` tokens are rejected with 401 (API2). Account locks after 5 failed attempts. |
| 7 | 2 | As the **security owner**, I want every endpoint to deny by default so that a forgotten attribute can't open a hole. | A fallback authorization policy requires an authenticated user. A test lists every endpoint and checks its policy (API5, API9). |
| 8 | 3 | As the **security owner**, I want every push scanned for secrets, vulnerable code, vulnerable dependencies and image CVEs so that issues are caught before deployment. | gitleaks, Semgrep, the NuGet vulnerable-package check and Trivy run in CI. High or critical findings fail the build. Each finding is logged in `docs/vuln-register.md`. |
| 9 | 4 | As an **adjuster**, I can request an AI summary of a claim with a list of missing information so that I can review claims faster. | PII is redacted before the LLM call. Output is validated against a JSON schema and labelled "AI-generated, verify before acting". 10 prompt-injection notes fail to change the output format (API10, LLM01). |
| 10 | 4 | As an **admin**, I can see daily claim counts by status so that I can track the workload. | `GET /api/v1/reports/claims-daily` reads from the `reporting` schema filled by the nightly ETL. Customers and adjusters get 403. |
