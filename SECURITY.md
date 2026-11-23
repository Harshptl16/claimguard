# Security policy

ClaimGuard is a personal learning project that uses synthetic data only. It is not a production system.

## Reporting a vulnerability

Please don't open a public issue. Use GitHub's **Report a vulnerability** button on the Security tab (private vulnerability reporting). I aim to respond within 7 days.

## How this project handles security

- Every control maps to the OWASP API Security Top 10 (2023) and has an automated test or scan. See the README's security table.
- Every push runs gitleaks, Semgrep, a NuGet vulnerable-package check and a Trivy image scan. Dependencies are updated manually through reviewed pull requests.
- High or critical findings fail the build. Each finding is triaged in [`docs/vuln-register.md`](docs/vuln-register.md) with its CVSS severity and a fix-or-accept decision.
- Secrets live in .NET user-secrets locally and Azure Key Vault in the cloud. Nothing secret is committed.
- No real personal data is ever used. PII is redacted before any text is sent to an LLM.
