# Applied-Devops---WS-26-27
# Hostpital Management - DevOps Project

## 1. Project overview
## 2. Requirement analysis
### 2.1 Functional requirements
-FR-01: The system shall allow receptionists to register a new patient. 

-FR-02: The system shall allow authorized staff to search for patients.

-FR-03: The system shall allow authorized staff to view patient information.

-FR-04: The system shall allow authorized staff to edit patient information.

-FR-05: The system shall allow authorized staff to deactivate patient records while preserving historical data.

-FR-06: The system shall allow administrators to register doctors.

-FR-07: The system shall allow administrators to update doctor information.

-FR-08: The system shall allow administrators to deactivate doctors.

-FR-09: The system shall store doctor specialization and availability information.

-FR-10: The system shall allow receptionists to schedule appointments.

-FR-11: The system shall prevent double-booking of doctors.

-FR-12: The system shall allow staff to update or cancel appointments.

-FR-13: Doctors shall be able to record diagnoses and visit notes.

-FR-14: Authorized medical staff shall be able to view previous medical records.

-FR-15: Users shall be required to log in before accessing protected functionality.

-FR-16: The system shall support receptionist, doctor, and administrator roles.

-FR-17: The system shall enforce role-based access control.

-FR-18: Doctors shall be able to export authorized data to CSV format.

### 2.2 Non-functional requirements
-NFR-01 Performance
The system shall process normal user requests within 2 seconds under the expected demonstration workload.

-NFR-02 Availability
The application shall automatically recover from application or container failures.

-NFR-03 Password Security
User passwords shall not be stored in plain text and shall be protected using a secure hashing mechanism.

-NFR-04 Access Security
All protected functionality shall require authentication and role-based authorization.

-NFR-05 Deployability
A successful change merged into the main branch shall automatically produce and publish a container image.

-NFR-06 Testability
Automated unit tests and end-to-end tests shall run on every pull request.

-NFR-07 Observability
Application and system metrics shall be available through a monitoring dashboard.

-NFR-08 Maintainability
The project shall use a documented branching strategy and a versioning scheme for software artifacts.

## 3. Roles and access control
### Receptionist

The receptionist manages patient records and appointments.
The receptionist can register, search, update and deactivate patients as well as schedule, update and cancel appointments.
The receptionist cannot access or modify medical notes.

### Doctor

The doctor can access information about assigned patients and appointments.
The doctor can record diagnoses and visit notes, view previous medical records and export authorized data to CSV.

### Administrator

The administrator manages doctors and user accounts.
The administrator can register, update and deactivate doctors and assign user roles.
The administrator does not create or modify medical records.

### RBAC Matrix
 
| Capability | Receptionist | Doctor | Administrator |
|------------|-------------|---------|---------------|
| Register, search and edit a patient | Yes | No | Yes |
| Deactivate a patient record | Yes | No | Yes |
| Register, update or deactivate a doctor | No | No | Yes |
| Schedule an appointment | Yes | No | Yes |
| View appointments | All | Own only | All |
| Update or cancel an appointment | Yes | Own only | Yes |
| Record diagnosis and visit notes | No | Yes | No |
| Read a patient's medical history | No | Own patients | No |
| Export authorised data to CSV | No | Yes | Yes |
| Manage user accounts and roles | No | No | Yes |

#### Justification

- Doctors may update or cancel only their own appointments in accordance with the principle of least privilege.
- Administrators may not access patient medical histories because access to sensitive medical data is restricted to authorized medical staff.
- Administrators may export authorized data to support administrative and auditing activities.

## 4. Product backlog

## US-20
As any staff member, I want to log in, so that I can access only the functionality permitted by my role.

## US-17
As an Administrator, I want to create user accounts and assign roles, so that users receive the correct permissions.

## US-1
As a Receptionist, I want to register a new patient, so that patient information can be stored in the system.

## US-2
As a Receptionist, I want to search for an existing patient, so that I can quickly access patient information.

## US-3
As a Receptionist, I want to edit a patient's contact details, so that patient information remains accurate and up to date.

## US-14
As an Administrator, I want to register a doctor with a specialisation, so that doctors can be managed in the system.

## US-15
As an Administrator, I want to update doctor details, so that doctor information remains accurate.

## US-16
As an Administrator, I want to deactivate an unavailable doctor, so that inactive doctors cannot be scheduled for appointments.

## US-5
As a Receptionist, I want to schedule an appointment, so that patients can be assigned to available doctors.

## US-6
As a Receptionist, I want to update or cancel an appointment, so that scheduling conflicts and patient changes can be handled.

## US-8
As a Doctor, I want to view my scheduled appointments, so that I know which patients I need to see.

## US-9
As a Doctor, I want to open the record of a patient assigned to me, so that I can review relevant patient information.

## US-10
As a Doctor, I want to record diagnoses and visit notes, so that patient treatments are documented.

## US-11
As a Doctor, I want to read the previous medical records of my patients, so that I can make informed medical decisions.

## US-4
As a Receptionist, I want to deactivate a patient record, so that inactive patients are no longer treated as active.

## US-18
As an Administrator, I want to deactivate a user account, so that former staff members can no longer access the system.

## US-12
As a Doctor, I want to export authorised data to CSV, so that I can create reports when needed.

## US-19
As an Administrator, I want to reset a user's password, so that account access can be restored when necessary.

## US-7
As a Receptionist, I want to filter appointments by date, so that I can find appointments more efficiently.

## US-13
As a Doctor, I want to search my appointments by patient name, so that I can quickly find a specific patient.

##US-21
As a Developer, I want automated tests in the pipeline, so that defects are detected early.

## Top Three Priorities
 
US-20 is the highest priority because all protected functionality requires authentication and role-based access control.
 
US-17 is second because users and roles must exist before permissions can be enforced after login.
 
US-1 is third because patient management is a core business function and most other hospital workflows depend on patient records being available.

### Acceptance Criteria - US-20

Given valid credentials, when a staff member submits the login form, then access to the system is granted.

Given invalid credentials, when a staff member submits the login form, then access is denied and an error message is displayed.

Given a logged-in user, when the dashboard is loaded, then only functionality allowed for that role is available.

### Acceptance Criteria - US-17

Given valid user information, when an administrator creates a new account, then the account is stored successfully.

Given a selected role, when an administrator assigns the role to a user, then the role is linked to the account.

Given an existing account, when the administrator views the user details, then the assigned role is displayed correctly.

### Acceptance Criteria - US-5

Given a doctor already has an appointment at a selected time, when a receptionist attempts to book another appointment for the same doctor, then the system rejects the booking.

Given an available doctor, when a receptionist creates an appointment, then the appointment is stored successfully.

Given a newly created appointment, when the appointment list is opened, then the appointment is displayed.

## 5. Process and ceremonies 
## Sprint Length
The project will use two-week sprints. Two-week sprints provide enough time to implement complete features while allowing regular feedback during the weekly lab sessions.

## Scrum Ceremonies
### Sprint Planning
 
When:
- Monday evening at the beginning of each sprint (30-60 minutes).
 
What I do:
- Review the product backlog.
- Select stories for the sprint.
- Define the sprint goal.
- Break stories into implementation tasks.
 
Produces:
- Sprint backlog.
- Sprint goal.
 
### Sprint Review
 
When:
- Friday before or during the lab session (approximately 30 minutes).
 
What I do:
- Demonstrate completed functionality.
- Verify completed stories against their acceptance criteria.
- Collect feedback from lecturers, tutors, and classmates.
 
Produces:
- Feedback on the current increment.
- Potential changes to backlog priorities.
 
### Sprint Retrospective
 
When:
- Friday after the sprint review (approximately 20 minutes).
 
What I do:
- Reflect on what went well and what problems occurred.
- Identify one improvement for the next sprint.
- Record retrospective notes in the repository.
 
Produces:
- One action item for the next sprint.
- Written retrospective notes.
 
### Daily Scrum Alternative
As a solo developer, I do not hold daily Scrum meetings.
Instead, I maintain a dated work log in the repository and update the project board whenever work is completed or priorities change.

## Planned Sprints
 
### Sprint 1 - Authentication and User Management

Deliverables:
- US-20 Login
- US-17 Create user accounts and assign roles
- Basic RBAC implementation
- CI pipeline setup

### Sprint 2 - Patient and Doctor Management

Deliverables:
- US-1 Register patient
- US-2 Search patient
- US-3 Edit patient details
- US-4 Deactivate patient
- US-14 Register doctor
- US-15 Update doctor details
- US-16 Deactivate doctor
 
### Sprint 3 - Appointments and Medical Records

Deliverables:
- US-5 Schedule appointment
- US-6 Update or cancel appointment
- US-8 View appointments
- US-9 Open patient records
- US-10 Record diagnoses and notes
- US-11 Read medical history
 
### Sprint 4 - Reporting and Finalisation

Deliverables:
- US-12 Export CSV
- US-7 Filter appointments
- US-13 Search appointments
- US-19 Reset password
- End-to-end testing
- Documentation updates
- Bug fixes and project polish

## Backlog Refinement Triggers
Backlog refinement is performed when one of the following situations occurs:
 
- A new requirement is identified.
- An existing requirement changes.
- A user story is too large to be completed within one sprint.
- A story is not completed during a sprint and must be re-estimated.
- A technical constraint or dependency is discovered during implementation.
- Feedback from a sprint review results in changes to priorities or functionality.


# 6. Epics

## E1 - Authentication and Access

**Description:** Login, user accounts, role assignment and access control.

**Stories:**
- US-20: Log in
- US-17: Create user accounts and assign roles
- US-18: Deactivate a user account
- US-19: Reset a user's password


## E2 - Patient Management

**Description:** Create, search, maintain and deactivate patient records.

**Stories:**
- US-1: Register a new patient
- US-2: Search for an existing patient
- US-3: Edit a patient's contact details
- US-4: Deactivate a patient record


## E3 - Doctor Management

**Description:** Manage doctors and their availability.

**Stories:**
- US-14: Register a doctor with a specialisation
- US-15: Update doctor details
- US-16: Deactivate an unavailable doctor


## E4 - Appointment Scheduling

**Description:** Create, manage and search appointments.

**Stories:**
- US-5: Schedule an appointment
- US-6: Update or cancel an appointment
- US-7: Filter appointments by date
- US-8: View my scheduled appointments
- US-13: Search my appointments by patient name


## E5 - Medical Records and Export

**Description:** Manage medical information and reporting.

**Stories:**
- US-9: Open the record of a patient assigned to me
- US-10: Record diagnoses and visit notes
- US-11: Read the previous medical records of my patients
- US-12: Export authorised data to CSV


## E6 - Platform and Pipeline

**Description:** Containerisation, CI/CD, automated testing and monitoring.
- US-21: As a Developer, I want automated tests in the pipeline, so that defects are detected early.

## User-Story decomposition into Tasks
### US-20: As any staff member, I want to log in, so that I can access only the functionality permitted by my role.
| ID | Task | Kind | Depends on |
|----|------|------|------------|
| T-20.1 | Create User entity and database migration | Code | - |
| T-20.2 | Implement authentication service | Code | T-20.1 |
| T-20.3 | Implement login endpoint | Code | T-20.2 |
| T-20.4 | Create login page | Code | T-20.3 |
| T-20.5 | Enforce role-based access control | Code | T-20.2 |
| T-20.6 | Automated tests for login and authorization | Test — automated | T-20.5 |
Blocked by: None

### US-17: As an Administrator, I want to create user accounts and assign roles, so that users receive the correct permissions.
| ID | Task | Kind | Depends on |
|----|------|------|------------|
| T-17.1 | Extend User entity with role information | Code | T-20.1 |
| T-17.2 | Create user management service | Code | T-17.1 |
| T-17.3 | Implement user creation endpoint | Code | T-17.2 |
| T-17.4 | Implement role assignment functionality | Code | T-17.3 |
| T-17.5 | Create administration interface for user management | Code | T-17.4 |
| T-17.6 | Automated tests for user creation and role assignment | Test — automated | T-17.4 |
| T-17.7 | Verify role permissions manually using multiple test accounts | Test — manual | T-17.5 |
Blocked by: US-20

### US-1: As a Receptionist, I want to register a new patient, so that patient information can be stored in the system.
| ID | Task | Kind | Depends on |
|----|------|------|------------|
| T-1.1 | Create Patient entity and database migration | Code | - |
| T-1.2 | Implement patient registration service | Code | T-1.1 |
| T-1.3 | Create patient registration endpoint | Code | T-1.2 |
| T-1.4 | Create patient registration form | Code | T-1.3 |
| T-1.5 | Add validation for mandatory patient fields | Code | T-1.4 |
| T-1.6 | Automated tests for patient registration | Test — automated | T-1.5 |
Blocked by: US-20

### US-2: As a Receptionist, I want to search for an existing patient, so that I can quickly access patient information.
| ID | Task | Kind | Depends on |
|----|------|------|------------|
| T-2.1 | Implement patient search service | Code | T-1.1 |
| T-2.2 | Create patient search endpoint | Code | T-2.1 |
| T-2.3 | Create search form and results table | Code | T-2.2 |
| T-2.4 | Add support for searching by patient name | Code | T-2.1 |
| T-2.5 | Automated tests for patient search | Test — automated | T-2.4 |
Blocked by: US-20, US-1

### US-3: As a Receptionist, I want to edit a patient's contact details, so that patient information remains accurate and up to date.
| ID | Task | Kind | Depends on |
|----|------|------|------------|
| T-3.1 | Create patient edit endpoint | Code | T-1.1 |
| T-3.2 | Create patient edit form | Code | T-3.1 |
| T-3.3 | Implement update functionality in the patient service | Code | T-3.1 |
| T-3.4 | Add validation for contact information | Code | T-3.3 |
| T-3.5 | Automated tests for patient updates | Test — automated | T-3.4 |
| T-3.6 | Manual verification of successful updates | Test — manual | T-3.2 |
Blocked by: US-20, US-1

### US-14: As an Administrator, I want to register a doctor with a specialisation, so that doctors can be managed in the system.
| ID | Task | Kind | Depends on |
|----|------|------|------------|
| T-14.1 | Create Doctor entity and database migration | Code | - |
| T-14.2 | Add specialisation attribute to doctor records | Code | T-14.1 |
| T-14.3 | Implement doctor registration service | Code | T-14.2 |
| T-14.4 | Create doctor registration endpoint | Code | T-14.3 |
| T-14.5 | Create doctor registration form | Code | T-14.4 |
| T-14.6 | Automated tests for doctor registration | Test — automated | T-14.4 |
Blocked by: US-20

### US-15: As an Administrator, I want to update doctor details, so that doctor information remains accurate.
| ID | Task | Kind | Depends on |
|----|------|------|------------|
| T-15.1 | Create doctor edit endpoint | Code | T-14.1 |
| T-15.2 | Create doctor edit form | Code | T-15.1 |
| T-15.3 | Implement doctor update functionality | Code | T-15.1 |
| T-15.4 | Add validation for updated doctor information | Code | T-15.3 |
| T-15.5 | Automated tests for doctor updates | Test — automated | T-15.4 |
Blocked by: US-20, US-14

### US-16: As an Administrator, I want to deactivate an unavailable doctor, so that inactive doctors cannot be scheduled for appointments.
| ID | Task | Kind | Depends on |
|----|------|------|------------|
| T-16.1 | Add active/inactive status to doctor records | Code | T-14.1 |
| T-16.2 | Implement doctor deactivation service | Code | T-16.1 |
| T-16.3 | Create doctor deactivation endpoint | Code | T-16.2 |
| T-16.4 | Prevent appointment booking for inactive doctors | Code | T-16.2 |
| T-16.5 | Automated tests for doctor deactivation rules | Test — automated | T-16.4 |
| T-16.6 | Manual verification that inactive doctors cannot be selected during appointment booking | Test — manual | T-16.4 |
Blocked by: US-20, US-14

## 7. Scaling the User-Storys
The project uses the Fibonacci scale (1, 2, 3, 5, 8, 13) for story point estimation. The increasing gaps between values reflect growing uncertainty and complexity, making it easier to compare user stories based on effort, risk, dependencies, and testing requirements rather than estimated hours.

| Points | Meaning |
|----------|----------|
| 1 | Trivial change with minimal implementation and testing effort. |
| 2 | Small feature with limited logic and few dependencies. |
| 3 | Standard story involving multiple tasks, validation, and testing. |
| 5 | Substantial feature involving multiple components, business rules, or additional testing. |
| 8 | Large or unfamiliar feature involving security, authorisation, or significant technical uncertainty. |
| 13 | Too large to estimate reliably and should be split into smaller stories before implementation. |

## Story Point Estimates
| ID | User Story | Points | What drives the estimate |
|-----|------------|---------|-------------------------|
| US-20 | Log in and access role-based functionality | 8 | Authentication, sessions, password handling, and role-based access control introduce significant uncertainty and security requirements. |
| US-17 | Create user accounts and assign roles | 5 | User management builds on authentication and directly affects the permission model. |
| US-1 | Register a new patient | 3 | Anchor story. Requires a form, validation, persistence, and testing. |
| US-2 | Search for an existing patient | 2 | Simple query and results view with limited business logic. |
| US-3 | Edit a patient's contact details | 3 | Similar complexity to patient registration but reuses existing patient data. |
| US-14 | Register a doctor with a specialisation | 2 | Similar to a basic create form with limited validation requirements. |
| US-15 | Update doctor details | 2 | Small update operation based on existing doctor records. |
| US-16 | Deactivate an unavailable doctor | 3 | Includes business rules because inactive doctors must not receive future appointments. |
| US-5 | Schedule an appointment | 5 | Involves relationships between patients and doctors and requires scheduling validation. |
| US-6 | Update or cancel an appointment | 3 | Extends appointment functionality with moderate business logic. |
| US-8 | View my scheduled appointments | 2 | Primarily a filtered list view for the logged-in doctor. |
| US-9 | Open the record of a patient assigned to me | 3 | Requires authorisation checks and access to patient information. |
| US-10 | Record diagnoses and visit notes | 3 | Form-based functionality with medical record persistence and validation. |
| US-11 | Read previous medical records | 3 | Requires controlled access to historical medical information. |
| US-4 | Deactivate a patient record | 2 | Simple status update with minimal business logic. |
| US-18 | Deactivate a user account | 2 | Small administrative operation building on the user-management functionality. |
| US-12 | Export authorised data to CSV | 3 | Export itself is straightforward, but access control must be enforced correctly. |
| US-19 | Reset a user's password | 3 | Security-sensitive functionality requiring testing and validation. |
| US-7 | Filter appointments by date | 1 | Simple filtering of existing appointment data. |
| US-13 | Search appointments by patient name | 1 | Small search enhancement to existing appointment functionality. |
| US-21 | As a Developer, I want automated tests in the pipeline, so that defects are detected early. | 5 | Requires CI integration, automated test execution, build configuration, and troubleshooting failing pipeline runs. More complex than a standard CRUD feature but less uncertain than implementing authentication and authorization. |

## 8. Sprint Backlog Outline
The following sprint plan is an initial forecast based on the estimated story points. It may change as the project progresses and actual velocity becomes known.

| Sprint | Goal | Stories | Points |
|---------|---------|---------|---------|
| Sprint 1 | Authentication and user management | US-20, US-17 | 13 |
| Sprint 2 | Patient and doctor management | US-1, US-2, US-3, US-4, US-14, US-15, US-16 | 15 |
| Sprint 3 | Appointment management and quality improvements | US-5, US-6, US-8, US-21 | 15 |
| Sprint 4 | Medical records, reporting, and project completion | US-9, US-10, US-11, US-12, US-18, US-19, US-7, US-13 | 19 |

## 9. Language, framework, database, test runner

