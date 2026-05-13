# Cross-Platform Requirements

Because the plan calls for Windows and Linux support, tests must explicitly cover:

- Path separators
- Case sensitivity differences
- Line endings
- File locking and watcher behavior
- Git CLI availability and failure modes
- Local file permissions

If the app uses a desktop wrapper later, add packaging tests for each target platform.

## Non-Functional Requirements

### Reliability

Test that the app never silently drops:

- Model edits
- View settings
- Trace links
- Git status information

### Responsiveness

Test that common actions remain interactive:

- Open repo
- Select node
- Search
- Drag a node
- Save a change

### Accessibility

At minimum, verify keyboard access for:

- Search
- Tree navigation
- Inspector focus
- Core editor commands

### Data safety

Test that a failed parse or failed write leaves the repository in a recoverable state.

## Risk-Based Priorities

Highest priority risks:

- Parser or writer drift causing semantic corruption
- Visual edits producing unstable text diffs
- Git branch comparison showing the wrong change set
- View persistence breaking after rename or move
- Windows/Linux behavior diverging

Medium priority risks:

- Performance degradation on larger repos
- Layout instability
- Edge cases in traceability and search

Lower priority until later phases:

- Advanced behavioral modeling
- Rich presentation polish
- Deep merge automation

## Practical Exit Criteria for the Project

The project is ready for serious use when all of the following are true:

- A supported SysML repo opens read-only and renders correctly
- A user can create and edit supported structure elements visually
- Saved text is stable enough for code review
- Branch differences are understandable semantically
- Views and traceability survive rename and reload cycles
- The same test suite passes on Windows and Linux
