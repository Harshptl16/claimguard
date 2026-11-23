#!/usr/bin/env bash
# Creates the initial backlog as GitHub issues with sprint and type labels.
# Requires the GitHub CLI, signed in:  gh auth login
# Run from the repo root after pushing:  ./scripts/create-issues.sh
set -euo pipefail

for label in "sprint-1:0e8a16" "sprint-2:1d76db" "sprint-3:5319e7" "sprint-4:fbca04" "user-story:c5def5" "security:b60205"; do
  gh label create "${label%%:*}" --color "${label##*:}" --force >/dev/null
done

create() { # title, sprint label, extra label (or ""), body
  local labels="user-story,$2"
  [[ -n "$3" ]] && labels="$labels,$3"
  gh issue create --title "$1" --label "$labels" --body "$4"
}

create "File a claim on my own policy" sprint-1 security \
"As a **customer**, I can file a claim against my own policy so that I can get reimbursed.

**Acceptance criteria**
- [ ] \`POST /api/v1/claims\` returns 201 with status \`Submitted\`
- [ ] Filing against another customer's policy returns 404 (API1)
- [ ] Client-sent \`status\` is ignored (API3)"

create "View my claim" sprint-1 security \
"As a **customer**, I can view my claim so that I can track its progress.

**Acceptance criteria**
- [ ] \`GET /api/v1/claims/{id}\` returns my claim
- [ ] Another customer's claim returns 404, not 403 (API1)"

create "Adjuster claim queue with pagination" sprint-1 security \
"As an **adjuster**, I can see a paginated queue of submitted claims so that I can work through them in order.

**Acceptance criteria**
- [ ] Filter by status
- [ ] Page size capped at 50 (API4)
- [ ] Customers get 403 (API5)"

create "Claim status state machine" sprint-1 "" \
"As an **adjuster**, I can move a claim through Submitted → In review → Approved/Denied so that decisions are recorded.

**Acceptance criteria**
- [ ] Invalid transitions return 409
- [ ] Every change writes an audit row (who, what, when)"

create "Add notes to a claim" sprint-1 security \
"As an **owner or adjuster**, I can add a note to a claim so that context stays with the claim.

**Acceptance criteria**
- [ ] Notes limited to 2,000 characters (API4)
- [ ] Stored and returned as plain text, never rendered"

create "Sign in with a short-lived JWT" sprint-2 security \
"As a **user**, I can sign in and get a short-lived token so that a stolen session can't be reused for long.

**Acceptance criteria**
- [ ] JWT expires in 15 minutes
- [ ] Expired, tampered and \`alg: none\` tokens return 401 (API2)
- [ ] Account locks after 5 failed attempts"

create "Deny by default on every endpoint" sprint-2 security \
"As the **security owner**, I want every endpoint to deny by default so that a forgotten attribute can't open a hole.

**Acceptance criteria**
- [ ] Fallback authorization policy requires an authenticated user
- [ ] A test enumerates every endpoint and checks its policy (API5, API9)"

create "DevSecOps scanning on every push" sprint-3 security \
"As the **security owner**, I want every push scanned so that issues are caught before deployment.

**Acceptance criteria**
- [ ] gitleaks, Semgrep, the NuGet vulnerable-package check and Trivy run in CI
- [ ] High or critical findings fail the build
- [ ] Each finding logged in \`docs/vuln-register.md\`"

create "AI claim summary with guardrails" sprint-4 security \
"As an **adjuster**, I can request an AI summary of a claim with a list of missing information so that I can review claims faster.

**Acceptance criteria**
- [ ] PII redacted before the LLM call
- [ ] Output validated against a JSON schema and labelled \"AI-generated, verify before acting\"
- [ ] 10 prompt-injection notes fail to change the output format (API10, LLM01)"

create "Daily claims report for admins" sprint-4 "" \
"As an **admin**, I can see daily claim counts by status so that I can track the workload.

**Acceptance criteria**
- [ ] \`GET /api/v1/reports/claims-daily\` reads from the \`reporting\` schema
- [ ] Customers and adjusters get 403"

echo "Done. Add the issues to your Projects board: gh project item-add <number> --owner @me --url <issue-url>, or use the board's 'Add item' button."
