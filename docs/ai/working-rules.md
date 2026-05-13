# Working Rules

These rules are for implementation work in this repository.

## Before Editing

- Read the product vision, AI guide, and the relevant decision docs.
- Identify the owning file before making a change.
- Check whether the change affects parser behavior, model schema, or write policy.
- Check whether the change supports or weakens a product vision pillar.

## During Editing

- Prefer the smallest coherent change set.
- Keep unrelated files untouched.
- Preserve existing formatting unless the task explicitly calls for reformatting.
- Update docs when implementation decisions change.
- Add or update a vision trace when a decision changes.

## After Editing

- Run the narrowest tests that prove the change.
- Verify round-trip behavior when parsing or saving is involved.
- Verify no unrelated files changed if the task was supposed to be localized.

## Escalation Rule

If a required implementation decision is still open, stop and surface the gap instead of inventing a default.

If a proposed change conflicts with the product vision, stop and surface the tradeoff before implementing it.
