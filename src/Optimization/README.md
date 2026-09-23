# Optimization

This project is the isolation boundary for CPU-heavy planning/optimization code.

Rules:
- Depends on Application, never on API, Worker, or Infrastructure.
- Solver-specific packages (for example OR-Tools) belong here.
- Domain/Application must not reference solver-specific types.
- Do not add a solver until the Planning input/output contract is defined from the real domain.
