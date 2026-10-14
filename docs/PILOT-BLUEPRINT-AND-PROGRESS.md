# School Transportation — Living Product Blueprint & Progress Tracker

> **Single working entry point | Version 1.0 | Updated: 2026-10-14**
>
> **Mission:** Build a pilot-ready school transportation product that one school can confidently try on selected real routes for days or weeks before the second semester.
>
> **Status:** Foundation exists. First transportation-request domain PR is drafted, not merged. No production-ready flow has been demonstrated.
>
> **Rule:** Never make another interview a prerequisite for continuing. Use existing discovery, explicit assumptions, synthetic scenarios, and later pilot feedback.

## How to use this file (keep it alive)

This is the one **living project dashboard** for why the product exists, discoveries, current scope, decisions, work progress, and next steps. Detailed source files remain authoritative and are linked below. Do not delete evidence or quietly rewrite historical conclusions.

- Every merged PR updates the **Progress board**, **Change log**, and—when relevant—the scenario checklist.
- Record new assumptions in the **Open questions** section, with a safe temporary behavior. Never label guesses as confirmed school policy.
- Status legend: `[x]` merged and verified; `[~]` in progress/draft; `[ ]` not implemented. Tests or pilot validation must be noted separately.
- Priorities: **P0** required for safe bounded pilot, **P1** useful but deferrable, **P2** later.
- On conflict: real validated evidence > `docs/CURRENT-DECISIONS.md` > BRD > UX / technical source documents > template conventions.
- Do not equate this roadmap with an assertion that branch protection, CI, security, or any integration is operational. Verify implementation.

## 1. Product vision and proof of success

A transportation manager should be able to transform messy paper/Excel/WhatsApp intake into accurate demand; independently verify morning pickup and afternoon drop-off locations; assign students and crews to buses across multiple school waves; review conflicts; publish accountable plans; and run selected routes with meaningful incident/fallback procedures.

**Success statement:** A real school manager says, “This is useful; we can trial it for a week or two,” and can actually operate a small set of routes safely with existing processes as backup.

**Not the objective:** Finishing dozens of endpoints, building a giant ERP, or pretending that algorithmic capacity estimates guarantee a seat or child safety.

**Pilot definition:** one school, small bounded fleet/routes, manager-approved plans, clearly assigned staff responsibilities, parallel run first, actual operational feedback, security/access controls, backup and manual fallback, explicit go/no-go criteria.

## 2. Recovered field discovery (retain these facts)

### 2.1 Observed or reported problems
- School processes vary. Admission may live in another system or on paper; transportation registration may be done by a registration office or transport manager.
- Staff rely on Excel and WhatsApp. Parent pins are requested over WhatsApp and transcribed into sheets; mismatches and human errors are common.
- Parents' supplied locations may be wrong, missing, ambiguous, changed, or differ between pickup and drop-off.
- Siblings can have distinct grades, genders, schedules, directions, locations and rules; family membership does not imply identical transport demand.
- One student may need AM only, PM only or both; do not create phantom demand for the unused direction.
- School start/end times vary. One real scenario discussed **two morning and three afternoon waves**; the same bus can serve more than one wave if turnaround and crew schedules permit.
- Example field scenario: approximately **20 buses with ~27 seats per bus**. A separate **1,200 students / 30 vehicles** scenario is a stress-test example, not an asserted customer fleet.
- Grade/gender/schedule/attendant constraints may influence grouping; exceptional human decisions can be needed.
- Route grouping and feasibility are difficult before the total demand is known. Schools may discuss or commit transport/pricing before actual routing certainty exists; inaccurate inputs cause underpricing and operational issues.
- Routes continue changing well after term start. A reported first-day run took about **three hours despite a prior dry run**.
- Bus attendants may require pickup/assignment coordination. Drivers, attendants, registration workers, transport managers, and guardians have different jobs.

### 2.2 Insights and selected product responses
- We **capture transportation demand**, not replace the entire school admissions/registration system.
- Intake can originate in manual entry, Excel import, guardian form/link, or future integration, but is normalized into one canonical model.
- WhatsApp can carry a secure link; WhatsApp messages and spreadsheets must not become the canonical data store.
- Provide human-verifiable source, review status, change provenance and edit history for each relevant location.
- Distinguish **data readiness** (sufficient verified inputs to plan) from **planning feasibility** (requires fleet, timetables, full demand and route constraints). No guaranteed seat at intake.
- Do not silently decline an enrollment or invent an approved exception; expose uncertainty and give authorized transport managers explicit decisions with reasons.
- Plan AM/PM independently and model actual waves, resources, ordered stops, versioned approval, daily execution and recovery.
- Prefer measurable usefulness through manual planning first. An optimizer later proposes candidates; a human publishes.

## 3. User roles, jobs, and UX boundaries

| Role | Job | Pilot-facing experience | Access principle |
|---|---|---|---|
| Registration staff | Capture/import/correct demand | Guided forms, import report, missing-data review | Cannot guarantee seats or approve critical exceptions |
| Guardian | Provide child-specific contact and location facts | Time-limited secure location capture or staff-assisted input | Only own authorized requests |
| Transport manager | Validate, assign, resolve conflicts, publish | Demand dashboard, risk inbox, maps, revision comparisons | Named accountable approver |
| Driver | Operate assigned trips | Mobile-friendly manifest, trip state, incidents | Assigned trips only |
| Bus attendant | Assist boarding and status recording | Assigned roster and approved events | Assigned trips only |
| School administrator | School settings, users and visibility | Controlled configuration and operational reports | Permissions audited |

**UX first:** document user handoffs, decision consequences, missing-data states, loading/error states, accessible maps, Arabic-first RTL where appropriate. Do not create a single giant Angular component to own all business logic. React Native is **later**, not mandatory for initial pilot if responsive web suffices.

## 4. Canonical workflow from intake through operations

1. **Capture request:** Guardian and one or more students; identity/contact, grade, gender, requested AM/PM service, independent pins, location sources. School may import from existing systems.
2. **Verify inputs:** Identify incomplete, duplicate-looking, suspicious or conflicting data; seek correction or explicitly verify location. Retain source and who/when.
3. **Calculate data readiness:** Needs attention vs ready for planning. This says **nothing** about guaranteed availability.
4. **Assess provisional risks:** Geography, possible overlap, schedule, fleet load, pricing or missing facts, as indications with confidence and evidence.
5. **Manager decision:** Correct/defer/review/decline per school policy, or accept permitted risk with identity/reason/time. Safety or legal hard limits are never overridable.
6. **Create planning scenario:** Configure waves, student demand, vehicles, usable seats, staff and turnaround. Build AM/PM plans, ordered stops, assignments and time estimates. Human can rearrange.
7. **Validate plan:** Unassigned students, physical capacity, crew/vehicle overlaps, absent verified pins, timing conflicts and policy violations visibly reported.
8. **Approve and publish:** Manager approves an explicit immutable revision; operational `TripPlan`/assigned manifests derive from it. Never silently overwrite history.
9. **Execute daily:** `TripExecution` records departures, stop events, boarding/drop-off if in pilot scope, delays and incidents with appropriate access and fallback.
10. **Replan transparently:** New revision and changed manifests, without modifying historical runs/executions.

## 5. Proposed domain boundaries (working—not all implemented)

| Area | Key concepts | Notes |
|---|---|---|
| Intake & demand | TransportRequest, Guardian, StudentTransportDemand, per-leg TransportLocation | Multi-child request; student needs independent |
| School setup | SchoolCalendar, Campus, SchoolWave, explicit school constraints | Never infer time from grade labels |
| Fleet & staff | Vehicle, usable capacity, Driver, Attendant, availability | Availability and true overlaps matter |
| Demand triage | Verification, MissingDataFlag, ReviewDecision, policy exception | Audit reviewer, source, reason, timestamps |
| Planning | PlanningRun, PlanRevision, Route, Stop, Assignment, constraint issue | History and candidates separate from operations |
| Published operations | TripPlan, TripExecution, trip events, manifest | Date/session instantiates execution; no destructive history |
| Cross-cutting | Authorization, AuditRecord, imports, export, notifications | Protect children's personal data |

**Important model distinctions:** a valid coordinate is not a verified coordinate; a verified pin is not serviceable; a ready request is not assigned; an assigned child is not automatically safe to transport; a candidate plan is not a published plan.

**Hard constraints:** non-overridable legal/safety rules, physical usable seating limits and actual resource/time overlap. **Configurable school policies:** schedules, gender/grade restrictions, service regions, maximum ride-time thresholds, sibling policy, attendant requirements. **Soft objectives:** travel time, stable routes and preferences. Unknown exact rules must remain configurable/provisional rather than hard-coded.

## 6. Technical architecture and guardrails

- .NET 10 **modular monolith**: Domain → Application → Infrastructure → API; modules organized inside each layer.
- Separate Worker for long CPU-bound planning tasks; specialized solver code stays in `src/Optimization` (not Domain/API/Infrastructure).
- PostgreSQL + PostGIS/NetTopologySuite for spatial data when needed; EF Core persistence with migrations.
- Angular administrative web UI; mobile-friendly trip views first, React Native only after a concrete requirement.
- SignalR for status/progress and appropriate notifications; **no Kafka/RabbitMQ** for the pilot absent a demonstrated need.
- Persist **PlanningRun** as a durable job/workflow; **outbox is for integration events**, never a giant solver snapshot or optimization queue.
- Repositories do not independently commit; application owns transactions through unit of work. Domain invariants and application validation are tested.
- Immutable published/operational history, minimum role access, audit trails, secrets handling, backup/restore rehearsal, disaster and manual fallback are necessary before live child data.
- Start with **one school per deployment**; multi-tenant orchestration is deferred. Provider decisions (maps/WhatsApp/payments) remain open.
- Do not require asynchronous optimizer availability for daily safe operation. Human approval remains the boundary.

Existing detailed sources: [Current Decisions](CURRENT-DECISIONS.md), [BRD v2](product/School-Transportation-BRD-v2.pdf), [UX Freeze v2](architecture/UX-Architecture-Freeze-v2.html), [Technical Architecture v3](architecture/School-Transportation-Technical-Architecture-v3.html).

## 7. Pilot scope / conscious exclusions

### P0 — actual trial capabilities
- School setup, user roles/authorization, wave/timetable settings, fleet/driver/attendant setup.
- Staff create/edit and robust audited Excel import with row errors and duplicate suggestions.
- Parent/guardian + children; independent AM/PM service and locations; verified-source handling and corrections.
- Demand dashboard and exception queue, manager review/decisions without silent acceptance.
- Manual route workspace with maps, ordered stops, AM/PM waves, assignments, physical capacity and schedule overlap checks.
- Revision review/publish, role-scoped manifests, basic trip statuses/events as needed for the selected routes.
- Export/printouts, audit records, backups, tested restore, permissions and operational fallback.

### P1 — improves demos, not initially blocking
- Time-limited parent self-service pin links if staff capture suffices initially; assisted route suggestions; route comparison; useful operational analytics.
- Optimization after we have a tested manual-planning baseline.

### P2 — consciously deferred
- Full admissions ERP, billing and automated pricing, payment gateways, hardware GPS/attendance, mandatory parent app, complex ETA/live tracking, native apps for every role, microservices/brokers, multi-tenant control plane.

## 8. Progress board — update with every PR

**As of 2026-10-14**. Checked means merged/verified—not just code written.

| Slice | Priority | Progress | Observable exit criterion | Evidence |
|---|---|---|---|---|
| 0. Repository foundation | P0 | PARTIAL | Local stack boots, migrations work, CI tests, authorization baseline | Template exists; solution rename PR #1 and Product cleanup PR #2 merged; running end-to-end stack not verified |
| 1a. Request domain | P0 | DRAFT | Multi-child per-leg model with tests green | [PR #3](https://github.com/SalehShalab87/School-Transportation/pull/3) draft, unmerged; tests not yet run |
| 1b. Request API/persistence | P0 | TODO | Create/read/correct persists against Postgres, audited verification | None |
| 1c. Request Angular UX/import | P0 | TODO | Staff can enter/import and review varied requests | None |
| 2. Readiness and manager decisions | P0 | TODO | No false seat guarantees, auditable risk actions | None |
| 3. Manual planning and publish | P0 | TODO | AM/PM, waves, assignments, validation, immutable revision, manifests | None |
| 4. Trip operations and pilot hardening | P0 | TODO | Selected trips operate with exports, backup/restore, fallback | None |
| 5. Assisted optimization | P1 | TODO | Worker suggests candidate without auto-publishing | None |
| 6. Bounded school pilot | P0 | TODO | One school uses selected routes, issues are tracked and measured | None |

**Current critical path:** review PR #3 -> resolve domain/persistence shape -> implement Slice 1 vertical workflow -> Slice 2 manager triage -> Slice 3 manual planning -> Slice 4 operations/pilot checks. Build the minimum of Slice 0 needed for each step; avoid infrastructure-only work for its own sake.

### Near-term backlog (ordered)
- [ ] Review and run PR #3 domain tests, adjust EF mapping requirements, correct design defects, merge to `dev` after review.
- [ ] Write a practical API contract and persistence mapping for TransportRequest, StudentTransportDemand and independent locations.
- [ ] Include who/when verified and source history; do not allow undocumented verification.
- [ ] Implement create/read/update/correct and a demand-readiness query with tests.
- [ ] Add minimal staff request UI with explicit missing/invalid states.
- [ ] Implement safe spreadsheet import preview, validation and per-row error display.
- [ ] Create synthetic demo fixtures with AM-only, PM-only, siblings, bad pins and duplicates.

## 9. Acceptance scenarios / regression checklist

- [ ] S01: Siblings have different grades/genders/schedules/pins without being forced into one route.
- [ ] S02: AM-only child never occupies an afternoon seat; PM-only never occupies morning.
- [ ] S03: Duplicate/malformed imported rows and uncertain locations return explicit errors; never guessed away.
- [ ] S04: Guardian changes a location after publication; old plan survives and responsible people see the impact.
- [ ] S05: Two AM and three PM waves; shared bus reuse requires feasible turnaround and crew schedule.
- [ ] S06: 28th child on a vehicle with 27 *usable* seats cannot silently publish.
- [ ] S07: Intake accepts an unconfirmed request without pretending a route or price is guaranteed.
- [ ] S08: Authorized manager records permitted ride-time exception with reason and history.
- [ ] S09: Unavailable driver/attendant triggers controlled replanning/manifest revision.
- [ ] S10: First-day delay despite dry run is recorded with actual operational events.
- [ ] S11: Export/printed fallback and tested backup restore allow safe continuity.
- [ ] S12: Unauthorized staff cannot browse arbitrary children's details or approve exceptions.

**Testing levels:** domain unit tests for invariants; application tests for review/auth/workflow; Postgres integration tests for persistence/constraints; API contract/security tests; end-to-end UX scenarios; supervised operational dry runs. Do not mark a scenario done solely because a domain unit test exists.

## 10. Open decisions — do not invent school policy

| Topic | Working direction | Confidence / action |
|---|---|---|
| Exact bell times, waves, shifts | Editable school setup, not grade-based guesses | Values unknown; synthetic fixtures until pilot |
| Grade/gender grouping | School-configurable constraints | Actual requirements differ |
| Capacity | Per-vehicle usable seat count and explicit limits | Approximate historic examples, confirm for pilot |
| Ride duration | Warning threshold and authorized exception if allowed | Exact maximum unknown |
| Serviceability | Provisional planning risk, not intake guarantee | Definition not frozen |
| Pricing | Record cost drivers if useful; no automatic fee commitment | Formula unknown; defer billing |
| Attendant pickup/availability | Real staff resource/availability constraints | Mechanics provisional |
| Sibling co-travel | Preference by default unless school says otherwise | Exact policy unknown |
| Location verification | Provenance, trusted verifier, timestamp, correction resets verification | Verification mechanism provisional |
| Maps/geocoding/WhatsApp | Provider adapter behind an interface | Provider and pricing not selected |
| Optimization objectives | Feasibility first; travel/time/stability weights later | Benchmark manual plans first |
| Driver offline | Export/print/manual fallback and outage rehearsal | Connectivity needs not measured |
| Safety/legal/privacy | School approval, least-privilege, retention and consent review before real data | Local requirements need checking before pilot |
| Route/plan publication approval | Named manager and immutable revision | Details and permission matrix to refine |

When school access is unavailable, **simulate known difficult cases**, preserve the provisional label, and keep a safe human override path. Pilot onboarding becomes the moment to configure facts, not an excuse to block all engineering.

## 11. Pilot execution and measurable evidence

**Demonstration data:** Build a realistic 40-request fixture for the first vertical slice; later generate a clearly synthetic ~1,200-student/30-vehicle load case (never mix with actual school data).

**Pilot progression:** synthetic full walkthrough → dry run with school-selected constraints → limited parallel operation with existing process → selected live routes only after go/no-go.

**Metrics to baseline and compare:** intake/import time; corrections and duplicate counts; requests ready/unassigned; manager review time; planning hours; schedule and capacity conflict count; on-time departures/arrivals; route changes; staff feedback and incident response.

**Non-negotiable go/no-go gates:** manager can execute core workflow without developer manipulating data; accurate manifests; restricted child-data access; no unresolved critical safety/security faults; backup/restore and fallback rehearsed; clear escalation owner; permission to use real data and suitable retention/deletion arrangements.

## 12. Change log / decision journal

| Date | Change | Status |
|---|---|---|
| 2026-09-23 | Repository foundation and source docs established | Historical |
| 2026-09-24 | PR #1 solution rename and PR #2 sample Product removal merged | Verified from commit history |
| 2026-10-14 | Pilot-ready outcome adopted instead of “feature-complete platform” | Product direction |
| 2026-10-14 | Existing discovery consolidated; more interviews not a precondition | Product direction |
| 2026-10-14 | PR #3 first request domain model/tests submitted to `dev` | **Draft; not merged or tested** |
| 2026-10-14 | This living blueprint introduced for tracking discovery and progress | Documentation PR |

**Update template for every meaningful change:** date | scenario/decision affected | why | PR/commit | verification performed | remaining risk | next owner/task.

## 13. Working rules for developers and AI agents

1. Read this document and `docs/CURRENT-DECISIONS.md` **before** implementing a module.
2. Never overwrite observed school facts with convenient technical assumptions.
3. Keep the product actor-first and pilot-focused; demonstrate one usable vertical workflow at a time.
4. Distinguish P0 pilot gates from nice-to-haves; do not demand native apps or sophisticated optimization on day one.
5. Implement deterministic invariants in Domain; keep school-specific rules configurable and traceable.
6. Record who approved what and why; historical published plans and actual trip events are immutable history.
7. Protect student location/contact information; don't commit personal data/secrets and don't use real children in demos without authorization.
8. Target PRs at `dev`; merge to `main` only from `dev` as repository policy. Verify actual branch protection instead of assuming.
9. No unreviewed automatic PR merges. Record test evidence honestly.
10. Update this blueprint's progress table and change log with each change. This is how we stop losing discovery.

---
**Next demonstrable outcome:** A staff member creates two siblings with distinct AM/PM requirements and supplied pins, corrects an inaccurate pin, sees readiness/review status, and reloads the persisted record from the Angular UI; no seat is promised.
