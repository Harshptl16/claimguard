# NIST SSDF (SP 800-218) mapping

> Completed in Sprint 5. Maps what this repo actually does to SSDF practices.

| SSDF practice | What ClaimGuard does | Evidence |
|---|---|---|
| PO.3 Implement supporting toolchains | GitHub Actions pipeline with security scanners | `.github/workflows/ci.yml` |
| PS.1 Protect all forms of code | Branch protection on `main`, PR-only merges | Repo settings |
| PW.4 Reuse well-secured software | NuGet vulnerable-package check in CI; pinned CI action versions | CI `build-test` job, ADR 0002 |
| PW.7 Review and analyze code | Semgrep SAST on every push | CI `sast` job |
| PW.8 Test executable code | Integration tests and ZAP API scan (Sprint 3) | `tests/`, CI |
| RV.1 Identify vulnerabilities | Trivy, gitleaks, NuGet vulnerable-package check | CI, Security tab |
| RV.2 Assess and remediate | Vulnerability register with CVSS and decisions | `docs/vuln-register.md` |
