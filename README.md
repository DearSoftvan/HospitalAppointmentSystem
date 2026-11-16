# Hospital Appointment System

An ASP.NET Core MVC hospital/clinic appointment management application created for a graduate-level **Object-Oriented Programming** course. The project supports healthcare organizations, their branches, staff schedules, patients and appointment approval workflows.

> **Project state:** Academic development snapshot. This documentation describes the features implemented during development. Run the checks below against the actual repository before treating any feature, test count, or GitHub submission as verified. The application is **not production-ready**.

## Assignment coverage

| Assignment requirement | Implementation / verification point |
| --- | --- |
| Hospital and clinic registration | `HealthcareOrganization`, admin configuration UI |
| Branches, departments and working hours | `Branch`, `Department`, `WorkingHours`, admin configuration and scheduling UI |
| Services with duration and price | `HealthcareService`, service management |
| Doctor and medical personnel registration | Abstract `Staff`; `Doctor`, `Nurse`, `LaboratoryTechnician`; staff management |
| Doctor specialties and offered services | `Doctor.Specialty`, `StaffService` association |
| Staff availability | `StaffAvailability`, scheduling management |
| Patient appointment booking | `Patient`, `Appointment`, MVC appointment forms |
| Appointment conflict prevention | `AppointmentService` and appointment repository checks |
| Appointment duration, price, provider, date | Appointment domain and persistence model |
| Approval mechanism | Automatic/manual `IApprovalStrategy`, admin approval, status history |
| Patient, staff, administrator access | ASP.NET Core Identity and role-protected controllers |
| Staff self-registration, administrator review | `StaffRegistrationRequest` and applicant/admin UI (verify end-to-end) |

**Important distinction:** The assignment recommends *at least three roles*. The implementation uses patient-facing authenticated access, a `Staff` role and an `Admin` role. Patient authorization is not necessarily modeled as a named `Patient` Identity role; inspect the code before presenting it that way.

## Technology stack

- C# and **.NET 10**
- ASP.NET Core MVC with Razor Views and Bootstrap
- Entity Framework Core with SQLite
- ASP.NET Core Identity for authentication and authorization
- xUnit unit and integration tests
- Git and GitHub

## Architecture

```text
Browser (HTML/CSS/JavaScript)
          |
          | HTTP(S)
          v
Web / Presentation (ASP.NET Core MVC Controllers, Views, ViewModels)
          |
          v
Application (appointment use cases, abstractions, approval strategies)
          |
          v
Domain (entities, invariants, business rules)

Infrastructure (EF Core, repositories, SQLite, Identity)
    implements persistence and identity services used by other layers
```

The **MVC pattern** is used in the Web project; the four projects provide additional separation of responsibilities. It is a *server-rendered web application*, not a separately deployed JavaScript SPA and API. A browser on another device can act as the client when the server is configured to listen on a reachable network interface. Localhost alone is not accessible from other devices.

### Solution layout

```text
HospitalAppointmentSystem/
├── HospitalAppointmentSystem.slnx
├── src/
│   ├── HospitalAppointmentSystem.Domain/
│   ├── HospitalAppointmentSystem.Application/
│   ├── HospitalAppointmentSystem.Infrastructure/
│   └── HospitalAppointmentSystem.Web/
├── tests/
│   ├── HospitalAppointmentSystem.UnitTests/
│   └── HospitalAppointmentSystem.IntegrationTests/
├── docs/
├── data/                      # Local SQLite file; do not commit
├── .gitignore
└── README.md
```

## OOP principles and patterns

| Principle | Example |
| --- | --- |
| **Abstraction** | Abstract `Staff` base class; repository and strategy interfaces |
| **Encapsulation** | Private setters and domain methods such as `Approve`, `Reject`, `Cancel`, `Complete` |
| **Inheritance** | `Doctor`, `Nurse`, `LaboratoryTechnician` derive from `Staff` |
| **Polymorphism** | Automatic and manual approval strategies implement `IApprovalStrategy` |
| **Interfaces** | `IAppointmentRepository`, `IApprovalStrategy` and other repository contracts |
| **Dependency injection** | Service and repository dependencies resolved through ASP.NET Core DI |
| **Persistence mapping** | EF Core staff inheritance mapping (TPH) |

`Patient` is currently a separate entity rather than a subclass of a shared `Person` class. This is a conscious model boundary, **not** a claim that every domain person uses a common base class. Some MVC controllers access `ApplicationDbContext` directly; the layer boundary is therefore not fully strict and remains a potential refactoring task.

## Main user workflows

### Patient

1. Register or sign in as a patient.
2. Choose branch, staff member, healthcare service and appointment time.
3. The booking flow checks staff-service assignment, operating hours, availability and conflicts.
4. The appointment is approved automatically or stays pending, depending on the healthcare organization's policy.
5. View personal appointments, status history and cancellation options.

### Administrator

1. Maintain organizations, branches, departments and healthcare services.
2. Add healthcare professionals and associate them with services.
3. Set branch working hours and staff availability.
4. Review pending appointments and pending staff applications.
5. Approve/reject an application, which creates a linked staff record and grants the `Staff` role.

### Healthcare staff

1. Apply through the separate staff registration form or obtain an administrator-created account.
2. Wait for administrator approval where applicable.
3. Sign in and open the role-protected staff portal to see appointments assigned to that staff member.

## Local setup (macOS / Windows / Linux)

### Prerequisites

- .NET SDK **10.x** (`dotnet --version`)
- Git
- SQLite CLI for optional inspection (`sqlite3`)
- HTTPS development certificate for secure local testing when applicable

### Restore, build and test

```bash
dotnet restore HospitalAppointmentSystem.slnx
dotnet build HospitalAppointmentSystem.slnx
dotnet test HospitalAppointmentSystem.slnx
```

Do **not** assume a particular number of passing tests without reviewing the actual output. Tests for registration approval, database transactions, authorization, and concurrent bookings require particular attention.

### Database migration

```bash
dotnet tool restore
dotnet ef database update \
  --project src/HospitalAppointmentSystem.Infrastructure \
  --startup-project src/HospitalAppointmentSystem.Web \
  --context ApplicationDbContext
```

The Web startup code constructs the SQLite database path under `data/hospital.db` at the solution root. Avoid committing the `.db` file; generate it locally using EF migrations.

### Development administrator

The project includes development-only admin seeding via environment variables. Define strong, local-only values before starting a fresh development database:

```bash
export HOSPITAL_ADMIN_EMAIL='admin@example.test'
export HOSPITAL_ADMIN_PASSWORD='REPLACE_WITH_A_STRONG_LOCAL_PASSWORD'
```

**Never commit passwords, `.env` files, private keys, live patient data, or real medical information.** Admin seeding behavior depends on the existing seed and startup configuration; review it before use in production.

### Run the application

```bash
dotnet run --project src/HospitalAppointmentSystem.Web
```

Use the URL printed by ASP.NET Core (the development environment previously used port 5073; do not assume the port is always the same).

### Selected MVC routes

| Route | Purpose |
| --- | --- |
| `/Account/Register` | Patient registration |
| `/Account/Login` | Sign in |
| `/Appointments/Create` | Create an appointment |
| `/Appointments` | Own patient appointments |
| `/Admin` | Pending appointment decisions |
| `/Configuration` | Organization/branch/service administration |
| `/StaffManagement` | Staff records and service assignments |
| `/ScheduleManagement` | Branch hours and staff availability |
| `/StaffAccounts` | Administrator-created staff accounts |
| `/StaffApply/Register` | Self-service staff application |
| `/StaffApply/Pending` | Applicant's review status |
| `/StaffApplications` | Administrator application review |
| `/StaffPortal` | Staff member's own appointments |

## Security and functional limitations

This is a course project, **not a deployed healthcare system**. Verify the following before relying on it outside a controlled demonstration:

- Cross-organization authorization: a global `Admin` role is not equivalent to per-hospital administrator permissions.
- Booking concurrency: sequential conflict checks alone do not guarantee conflict-free simultaneous reservations.
- Existing controllers that access EF Core directly should be reviewed for consistent authorization and business validation.
- User activation/role changes may require a new sign-in for the browser's authentication cookie to reflect the change.
- Staff approval transaction rollback and duplicate applications require integration testing under realistic conditions.
- Departments and healthcare services are modeled, but any required department-to-service UI mapping should be verified separately.
- Email ownership confirmation, invitation-based password creation, audit logging, HTTPS configuration, data protection and patient privacy controls need further work before production usage.
- The staff portal shows appointment information; this is not a clinical record system.

## Acceptance checklist

- [ ] `dotnet build HospitalAppointmentSystem.slnx` succeeds
- [ ] `dotnet test HospitalAppointmentSystem.slnx` succeeds
- [ ] No secrets or SQLite database files are staged or tracked
- [ ] Automatic and manual booking approval scenarios work
- [ ] Conflicting bookings are rejected
- [ ] Unapproved applicant is denied `/StaffPortal`
- [ ] Administrator approval creates a single linked staff account and adds the `Staff` role
- [ ] Applicant logs out and back in, then opens own staff portal
- [ ] Administrator pages reject non-admin users
- [ ] GitHub has at least **10 commits made on different calendar days**, per the assignment
- [ ] Project and presentation are uploaded to **SABIS** before the applicable deadline
- [ ] Presentation attendance requirements are met

## Course submission and versioning

Assignment option: **A — Hospital/Clinic Appointment System**. The instructor requires GitHub publication, SABIS submission, at least **10 commits on different days**, and a short presentation. A backup branch is useful for preserving a snapshot but **does not substitute** for commits on different days.

To preserve a tested snapshot after committing it on `main`, create a dated branch pointing at the same commit. Never force-push or rewrite commit dates to meet the assignment.

## Possible future improvements

- Fine-grained hospital/branch administrator roles and policies
- Application services for staff applications and a stronger controller/application separation
- Race-safe booking constraints and concurrency tests
- Email verification and invitations for administrative staff creation
- Richer service-to-department management
- Security, authorization and end-to-end test coverage
- Optional shared `Person` abstraction **only if justified by domain invariants and EF mapping**

## Academic context

Object-Oriented Programming graduate course, Sakarya University. The project emphasizes explicit business models, domain rules, inheritance, interfaces and polymorphic approval policies in a practical appointment scheduling application.
