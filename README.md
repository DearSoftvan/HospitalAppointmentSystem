# Hospital Appointment System

## Project Overview

HospitalAppointmentSystem is a web-based hospital and clinic appointment management system developed as an academic project for the Object-Oriented Programming graduate course.

The system is designed to support both individual healthcare institutions and multi-branch hospital organizations. It aims to provide a structured platform for managing healthcare services, medical staff, patient appointments, and institutional appointment approval policies.

## Project Objectives

- Apply object-oriented programming principles in a real-world software application.
- Develop a maintainable and modular software architecture.
- Support multiple hospitals, clinics, and branches.
- Manage healthcare services, staff availability, and patient appointments.
- Prevent appointment scheduling conflicts.
- Support automatic and manual appointment approval policies.
- Implement role-based authentication and authorization.
- Verify business rules through unit and integration testing.

## Technology Stack

| Technology | Purpose |
|---|---|
| C# | Programming language |
| .NET 10 | Application development platform |
| ASP.NET Core MVC | Web application framework |
| Entity Framework Core | Object-relational mapping |
| SQLite | Relational database |
| ASP.NET Core Identity | Authentication and authorization |
| Razor Views | Server-side user interface |
| Bootstrap | Frontend styling |
| xUnit | Unit and integration testing |
| Git | Version control |
| GitHub | Source code hosting |

## Software Architecture

The project follows a four-layer architecture that separates business logic, application operations, infrastructure, and presentation.

### Domain Layer

Contains domain entities, business rules, enumerations, and core domain abstractions. The Domain layer is independent of the other application layers.

### Application Layer

Contains application services, use cases, data transfer objects, and repository abstractions. It coordinates domain operations and implements application workflows.

### Infrastructure Layer

Provides database access, Entity Framework Core configurations, repository implementations, and identity infrastructure. The planned persistence design uses Table-per-Hierarchy (TPH) inheritance mapping.

### Web Layer

Provides the ASP.NET Core MVC presentation layer, controllers, views, and dependency injection configuration.

## Design Patterns and OOP Principles

The project is designed to demonstrate:

- Encapsulation
- Abstraction
- Inheritance
- Polymorphism
- Composition
- SOLID principles
- Dependency Injection
- Repository Pattern
- Strategy Pattern

The Strategy Pattern will be used to support institution-specific automatic and manual appointment approval policies.

The Repository Pattern will be used to abstract data access operations from the application layer.

## Project Structure

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
├── .gitignore
└── README.md
```

## Project Dependencies

```text
Application      --> Domain
Infrastructure   --> Application, Domain
Web              --> Application, Infrastructure

UnitTests        --> Domain, Application
IntegrationTests --> Web, Infrastructure, Application, Domain
```

## Planned Features

- Hospital and clinic management
- Multi-branch healthcare institution support
- Department management
- Healthcare service management
- Doctor and medical staff management
- Staff availability management
- Patient registration and authentication
- Appointment scheduling
- Appointment conflict detection
- Automatic and manual appointment approval
- Role-based authorization

## Getting Started

### Prerequisites

- .NET 10 SDK
- Git
- Visual Studio Code or another compatible IDE

### Restore Dependencies

```bash
dotnet restore HospitalAppointmentSystem.slnx
```

### Build the Solution

```bash
dotnet build HospitalAppointmentSystem.slnx
```

### Run the Web Application

```bash
dotnet run --project src/HospitalAppointmentSystem.Web/HospitalAppointmentSystem.Web.csproj
```

Open the local URL displayed in the terminal.

### Run Tests

```bash
dotnet test HospitalAppointmentSystem.slnx
```

## Development Status

The project is currently under active development.

### Completed

- Requirements analysis
- UML class design
- Database design
- Architectural decisions
- Development environment setup
- Four-layer solution structure
- Unit and integration test project setup
- Project dependency configuration
- Initial solution build verification (Release)

### Upcoming

- Domain entity implementation
- Database context and entity configurations
- Repository implementations
- Appointment management services
- Appointment approval strategies
- Identity and role-based authorization
- MVC user interfaces
- Business rule testing
- Integration testing

## Academic Context

This project is developed as part of the Object-Oriented Programming graduate course at Sakarya University.

The primary objective is to demonstrate object-oriented software design and implementation through a practical healthcare appointment management application.
