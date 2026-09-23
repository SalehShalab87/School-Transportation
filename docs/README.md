# Product & Architecture Documents

These documents travel with the codebase and are the design baseline for the School Transportation SaaS.

## Source documents

- `product/School-Transportation-BRD-v2.pdf` — business requirements baseline.
- `architecture/UX-Architecture-Freeze-v2.html` — UX and technical architecture baseline.

## Important precedence rule

These source documents are preserved unchanged. They contain some decisions that were later challenged during product/domain discovery. Do not silently treat every statement in them as a frozen implementation requirement.

Use this precedence when there is a conflict:

1. Real-world validated evidence
2. Latest explicitly recorded project decisions (`CURRENT-DECISIONS.md`)
3. BRD
4. UX / architecture freeze
5. Code/template conventions

Open items stay open. Do not invent missing business rules merely to make implementation convenient.

- `architecture/School-Transportation-Technical-Architecture-v3.html` — detailed technical architecture baseline (v3). Where it conflicts with `CURRENT-DECISIONS.md`, the latter records the newer project decision until the architecture document is revised.
