# Branching Strategy & Delivery Workflow

## 1. Chosen Strategy & Rationale
We selected a **GitHub Flow / Feature Branching** approach (with elements of Trunk-Based Development) over Git Flow. This decision is justified by three key project constraints:

* **Team Size & Overhead:** As a solo developer (and small project team), the multi-branch maintenance overhead of Git Flow (e.g., maintaining long-lived `develop`, `release`, and `hotfix` branches) introduces unnecessary merge synchronization without adding safety.
* **Release Frequency:** The project follows continuous integration and frequent small deliveries directly to production. Short-lived branches merging straight into `main` keep iteration speed high.
* **Test Maturity & CI/CD:** Automated GitHub Actions run validation, linting, and tests on every branch. Merging is gate-kept by automated checks rather than manual staging periods.

---

## 2. Branch Types & Naming Conventions

### Permanent Branch
* `main`: The only permanent, production-ready branch. It is protected against direct pushes and broken builds.

### Short-Lived Branches
All work happens on temporary branches that are deleted immediately after merging.

| Branch Type | Naming Pattern | Example | Purpose |
| :--- | :--- | :--- | :--- |
| Feature | `feat/<issue-id>-<short-description>` | `feat/12-user-auth` | New application functionality |
| Bugfix / Fix | `fix/<issue-id>-<short-description>` | `fix/34-token-expiry` | Addressing bugs or unexpected issues |
| Chore / Infra | `chore/<short-description>` | `chore/update-dependencies` | Tooling, CI config, or refactoring |
| Documentation | `docs/<short-description>` | `docs/api-endpoints` | Updates to repository documentation |

---

## 3. Merge Rules & Self-Review Policy

### Pull Request & Review Process
Direct pushes to `main` are strictly forbidden. All changes must enter `main` via Pull Requests.
* **Documented Self-Review:** Because this project is developed individually, all Pull Requests require a mandatory self-review prior to merging. Before clicking merge, the author must explicitly verify:
  1. The PR diff contains only changes relevant to the ticket/issue.
  2. No sensitive data, secrets, or temporary debug logs are staged.
  3. All local unit/integration tests pass cleanly.
* **Merge Mechanism:** **Squash and Merge** is the default merge method. This combines local, intermediate commits into a single atomic commit on `main`, keeping the trunk history clean and linear.

### Required Checks & Protection
* All CI build and test pipelines must pass (green status).
* Branches must be up-to-date with `main` before merging.

---

## 4. Release Rules & Semantic Versioning

* **Versioning Scheme:** Semantic Versioning (`vMAJOR.MINOR.PATCH`):
  * `MAJOR`: Breaking changes or incompatible API shifts.
  * `MINOR`: Backward-compatible new features.
  * `PATCH`: Backward-compatible bug fixes.
* **Tagging:** Git tags are created directly on `main` upon merging releases:
  * Tags are annotated (`git tag -a v1.0.0 -m "release: version 1.0.0"`).
  * Releases and deployment triggers are bound to these tags.

---

## 5. Conflict Resolution Policy

* **Responsibility:** The author of the Feature PR is strictly responsible for resolving merge conflicts.
* **Resolution Method:** 
  * Rebase on `origin/main` (`git fetch origin && git rebase origin/main`).
  * Conflicts are resolved locally in VS Code using `zdiff3` conflict markers for full ancestor context.
  * Once resolved, the branch is updated using `git push --force-with-lease`.
* **Ambiguous Logic:** If a conflict involves domain logic where the baseline cannot be determined trivially, the code must be tested against unit test suites before pushing.

---

## 6. Commit Graph & Workflow Diagram

```text
main           *---*-----------------------* (v1.0.0)
                    \                     /
feat/auth            *---*---* (Squash) -/
                              \
fix/login                      *---* (Rebased)