# Testing Strategy — ClinicApi

This document maps the user stories from Epic E1 (*Authentication and Access*) to the automated and manual tests covering them, and documents the coverage gate enforced in CI.

## User Story Test Matrix

| User story | Unit | Integration | E2E | Manual |
| :--- | :--- | :--- | :--- | :--- |
| **US-17** Create user accounts and assign roles | Email normalization, password length boundary (7 vs. 8 chars), role allow-list, duplicate email check (`UserAuthServiceTests`) | `POST /users` → `201 Created`, invalid role → `400 Bad Request`, duplicate email → `409 Conflict` (`ClinicApiIntegrationTests`) | Step 1 of `FullUserAuthLifecycle_AgainstRunningContainer_WorksEndToEnd` | Verify admin UI form wording (No UI yet — API-only thin slice) |
| **US-18** Deactivate a user account | Sets `IsActive = false`, blocks login with `AccountDeactivatedException`, unknown ID → `KeyNotFoundException` | `POST /users/{id}/deactivate` → `200 OK`, subsequent login → `403 Forbidden` | Step 4 of `FullUserAuthLifecycle_AgainstRunningContainer_WorksEndToEnd` | Verify deactivated user sees clear lockout message (No UI yet) |
| **US-19** Reset a user's password | Generates temporary password via stub, invalidates old password, blocks reset on deactivated user | `POST /users/{id}/reset-password` → `200 OK`, old password → `401`, temporary password → `200` | Step 3 of `FullUserAuthLifecycle_AgainstRunningContainer_WorksEndToEnd` | Verify password reset email/notification formatting (No notification service yet) |
| **US-20** Log in | Valid credentials return token, unknown email or wrong password throws `UnauthorizedAccessException` | `POST /auth/login` → `200 OK` with token, wrong password → `401 Unauthorized` | Step 2 of `FullUserAuthLifecycle_AgainstRunningContainer_WorksEndToEnd` | Exploratory check of login page UX (No UI yet) |

## Why Certain Test Types Are Omitted
* **No UI tests yet:** The current implementation is an API-only vertical slice for Epic E1; there is no frontend yet.
* **No Testcontainers yet:** The thin slice uses an in-memory `IUserRepository` (`InMemoryUserRepository`) without an external database. Once a relational database is introduced, `ClinicApiIntegrationTests` will switch to Testcontainers.

## Coverage Gate
* **Threshold:** **80% line coverage** on the domain layer (`[ClinicApi]ClinicApi.Domain.*`), enforced by `coverlet.msbuild` in the `unit` CI job.
* **Justification:** The domain layer contains all core security and business rules (email/password validation, role enforcement, account deactivation, and password reset). Measuring coverage specifically on `Domain` keeps the gate meaningful without rewarding boilerplate HTTP wiring in `Program.cs`, which is already verified by integration and E2E tests.