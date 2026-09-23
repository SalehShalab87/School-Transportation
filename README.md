# Backend Template - .NET 10 Clean Architecture (Fixed Architecture Edition)

A reusable backend foundation aligned with the architecture decisions for the School Transportation Solution. The `Product` module is intentionally only a **sample vertical slice**: use the same structure for `Students`, `Registration`, `Fleet`, `Planning`, `Trips`, `Attendance`, `Tracking`, `Notifications`, `Billing`, and `Reporting`.

## Product documentation

The business and UX/technical design baselines are versioned with this repository under [`docs/`](docs/). Start with [`docs/README.md`](docs/README.md) and [`docs/CURRENT-DECISIONS.md`](docs/CURRENT-DECISIONS.md) before implementing a module; later validated decisions may qualify older BRD/UX assumptions.


## Architecture

```text
Domain
  ^
  |
Application
  ^
  |
Infrastructure      Optimization
      ^                 ^
      |                 |
     API              Worker
```

The solution is a **module-first modular monolith inside Clean Architecture layers**. Modules repeat inside each layer rather than becoming dozens of projects on day one.

```text
src/
  Domain/
    Common/
    Products/               # sample module
  Application/
    Common/
    Products/
  Infrastructure/
    Persistence/
    Messaging/
    Products/
  Optimization/             # CPU-heavy solver boundary; solver packages stay here
  API/                      # HTTP host
  Worker/                   # background host (outbox + future planning jobs)
```

## What was fixed from the original template

- `Guid`/PostgreSQL `uuid` IDs instead of GUID strings and Oracle-oriented `IndexId`.
- PostgreSQL-only persistence; SQL Server, Oracle and EF InMemory production wiring removed.
- PostGIS-ready Npgsql configuration (`UseNetTopologySuite`).
- Rich aggregate example with private setters, invariants and explicit behavior.
- Domain events are framework-free in Domain and adapted to MediatR in Application/Infrastructure.
- Domain event handlers added.
- Integration events + transactional outbox added.
- Repositories no longer call `SaveChangesAsync`.
- `IUnitOfWork` owns the command transaction boundary.
- CQRS read side uses an optimized read service with `AsNoTracking` projections.
- FluentValidation is registered and runs through a MediatR pipeline behavior.
- Logging pipeline behavior added.
- Global API exception handling and ProblemDetails added.
- Result errors carry semantic types so NotFound maps to HTTP 404, Conflict to 409, etc.
- CORS is configuration-driven instead of `AllowAnyOrigin`.
- Separate Worker host added for outbox and future planning jobs.
- Dedicated Optimization project isolates CPU-heavy solver code and future OR-Tools dependencies from Domain/Application/Infrastructure.
- EF configuration split into `IEntityTypeConfiguration<T>` classes.
- Soft delete is opt-in (`ISoftDeletable`), not a property forced onto every entity.
- Real PostgreSQL integration tests use Testcontainers; EF InMemory is not used to pretend to be PostgreSQL.
- Architecture tests protect Clean Architecture dependencies.
- Docker Compose includes a PostGIS-enabled PostgreSQL instance.

## Command-side flow

```text
HTTP endpoint
   -> MediatR command
   -> LoggingBehavior
   -> ValidationBehavior
   -> Command handler
   -> Aggregate / domain rules
   -> Repository (track only)
   -> IUnitOfWork.SaveChangesAsync()
        -> dispatch domain events
        -> event handlers may enqueue outbox messages
        -> one EF SaveChanges transaction
```

## Domain event -> outbox flow

```text
Aggregate state change
   -> IDomainEvent
   -> UnitOfWork
   -> IDomainEventDispatcher
   -> MediatR DomainEventNotification<T>
   -> DomainEventHandler<T>
   -> IOutboxWriter
   -> outbox_messages (same DB transaction)

Worker
   -> IOutboxProcessor
   -> deserialize integration event
   -> MediatR publish
   -> integration event handlers
```

Delivery is **at-least-once**. Integration-event handlers must therefore be idempotent. A real broker can later replace the in-process integration-event publisher without changing domain aggregates.

## Query-side flow

```text
HTTP endpoint
   -> MediatR query
   -> query handler
   -> IProductReadService
   -> EF Core AsNoTracking projection
   -> DTO
```

Do not load a large aggregate just to render a dashboard.

## Database

The template intentionally targets one database technology:

- PostgreSQL
- PostGIS / NetTopologySuite-ready provider
- EF Core 10

Run locally:

```bash
docker compose up -d
```

Then create migrations from the repository root (when the .NET SDK is installed):

```bash
dotnet ef migrations add InitialCreate \
  --project src/Infrastructure \
  --startup-project src/API

dotnet ef database update \
  --project src/Infrastructure \
  --startup-project src/API
```

Use environment variables or secret management for production credentials. The checked-in connection string is for local development only.

## Tests

```text
tests/
  Domain.UnitTests/
  Application.UnitTests/
  Infrastructure.IntegrationTests/
  API.IntegrationTests/
  Architecture.Tests/
  Optimization.UnitTests/
```

`Infrastructure.IntegrationTests` and `API.IntegrationTests` require Docker because they run against a real `postgis/postgis` container.

Run:

```bash
dotnet test
```

## How to add a real business module

For example `Planning`:

```text
Domain/Planning/
  Aggregates/
  Entities/
  ValueObjects/
  Rules/
  Events/

Application/Planning/
  Commands/
  Queries/
  EventHandlers/
  Abstractions/

Infrastructure/Planning/
  Persistence/
  Routing/

API/Endpoints/PlanningEndpoints.cs
```

For the School Transportation Solution, the Worker can later host long-running planning jobs while the API only submits and observes planning runs. The actual solver implementation belongs in `Optimization`, not in the Worker or Infrastructure.

A planning job (`PlanningRun`) is its own persisted workflow/job concept; **do not use the outbox as the optimization job queue or store large planning snapshots in outbox JSON**. The outbox remains for durable integration events such as `PlanningCompleted` or `PlanPublished`.

## Important conventions

1. **Domain owns invariants.** FluentValidation validates requests; it does not replace domain rules.
2. **Repositories never commit.** One application command chooses the transaction boundary through `IUnitOfWork`.
3. **Queries do not need repositories.** Use purpose-built read services/projections where useful.
4. **Domain events are facts**, e.g. `PlanApproved`, not commands like `SendWhatsAppNow`.
5. **Integration events are durable contracts** and go through the outbox.
6. **Do not turn Common into a dumping ground.** Only genuinely cross-cutting primitives belong there.
7. **Do not force soft delete onto immutable history**, such as attendance events, GPS history, audit logs, payments or planning history.
8. **Keep API and Worker thin.** Business rules live in Domain/Application.
9. **CPU-heavy optimization belongs in `Optimization`.** OR-Tools and solver-specific types must not leak into Domain/Application/Infrastructure/API. External routing/map adapters may still belong in Infrastructure.
10. **Planning jobs are not outbox messages.** Persist `PlanningRun` separately; use outbox for durable integration events.
11. **Architecture tests are part of the design**, especially because modules are folder boundaries rather than separate assemblies.

## Notes

- Package versions are pinned for reproducibility.
- The template has been structurally reviewed in this environment, but the environment used to prepare it did not include the .NET SDK, so run `dotnet restore`, `dotnet build`, and `dotnet test` on your development machine/CI as the final compile gate.
