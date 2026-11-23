# ADR 0002: Pin third-party CI actions

**Status:** Accepted · 2026-11-23

## Context
In March 2026, an attacker force-pushed 76 of 77 version tags of `aquasecurity/trivy-action` to credential-stealing malware (CVE-2026-33634). Any workflow that referenced a moving tag ran the attacker's code with access to its secrets.

## Decision
- Reference third-party actions by exact release (`@v0.36.0`), never by `@master` or `@latest`.
- In Sprint 3, pin each one to a full commit SHA with the version in a comment, e.g. `uses: aquasecurity/trivy-action@<sha> # v0.36.0`.
- Action versions are updated by hand in a reviewed pull request, after reading the release notes.
- The workflow's default `GITHUB_TOKEN` permission is `contents: read`.

## Consequences
Updates need a PR review, but a compromised tag can no longer change what the pipeline runs.
