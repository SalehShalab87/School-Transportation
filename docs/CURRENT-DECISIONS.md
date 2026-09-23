# Current Project Decisions

This file records decisions made after BRD v2 / UX Architecture Freeze v2. It does not rewrite those source documents; it identifies where later reasoning supersedes or qualifies them.

## 1. Product boundary

The school may keep its existing admission/registration process. Transportation demand may enter through staff entry, import, parent capture, or future school-system integration. Once inside our boundary, transportation data must have one canonical representation.

## 2. Registration-time certainty

Do **not** promise final transportation feasibility or a guaranteed bus/seat merely because an individual request has a valid location or because static fleet capacity appears sufficient.

A new deployment may have no historical routes or complete demand. Geographic clustering, future registrations, schedules, multi-wave use, turnaround, and resource calendars can materially change feasibility.

The safe early distinction is:

- **Data readiness**: enough trustworthy information exists for the request to participate in planning.
- **Planning feasibility**: evaluated with a sufficiently complete planning population, fleet, schedules, and constraints.

BRD v2 statements requiring definitive feasibility/seat confirmation before registration therefore remain subject to business validation rather than being treated as a frozen invariant.

## 3. Transport demand / siblings

A request may contain one or more students (for example siblings). Each student can have independent grade, gender, and transport direction requirements.

Known V1 candidate data:

- Parent/guardian contact
- One or more students
- Student name
- Grade
- Gender
- Morning transport required? If yes, pickup map pin required
- Afternoon transport required? If yes, drop-off map pin required

Pickup and drop-off may differ. Siblings may share locations for UX convenience, but the domain must not assume they always have identical transport requirements.

## 4. Rules and constraints

Not all planning rules are equal:

- Hard/system constraints cannot be violated (for example physical capacity and true resource overlap).
- School/business constraints come from validated school policy and schedules.
- Soft preferences/objectives may be relaxed when necessary (for example keeping siblings together, if validated as a preference rather than a hard rule).

The optimizer consumes explicit operational facts/constraints. It must not hard-code business labels such as `Grade == 1` to infer times.

## 5. Planning versus operations

Preserve the distinction already present in the UX architecture:

- `PlanningRun` / revision = planning history and generated/reviewed candidate state.
- Approved plan produces operational `TripPlan` definitions.
- A date/session creates `TripExecution` state.
- Daily operational recovery must not rewrite historical PlanningRuns.

## 6. Optimization project boundary

The current backend template supersedes the UX v2 diagram that places the OR-Tools adapter under Infrastructure.

Current solution boundary:

```text
Domain
  ^
Application
  ^             ^
Infrastructure  Optimization
  ^             ^
API           Worker
```

Solver-specific code and packages (OR-Tools if/when selected) belong in `src/Optimization`. Domain and Application must not depend on solver/provider details.

## 7. Worker and durable jobs

The Worker is a host for long-running/background application work. Keep optimization job state separate from the transactional outbox.

- A future `PlanningRun`/planning-job model owns durable planning execution state.
- The transactional outbox is for durable external/cross-module event delivery and side effects.
- Do not serialize a huge planning snapshot into the outbox and use it as a job queue.

No Kafka/RabbitMQ/message broker is introduced without demonstrated need.

## 8. Still open — do not invent

Examples include:

- Exact serviceability definition
- Exact pricing formula
- School schedule model (grade/class/shift/campus relationship)
- Exact sibling-together policy
- Solver/decomposition strategy
- Turnaround model
- Maps/WhatsApp/payment providers and capabilities
- Attendance hardware/protocol
- Detailed authorization/reporting/retention rules

These must be validated through product discovery or representative prototypes before being frozen.
