# MVP Coverage

The MVP supports the following SysML v2 elements:

- Package
- Part definition
- Part usage
- Port
- Connection
- Requirement
- Satisfy / trace relationship

The initial test suite should focus on those elements first.
The minimum gate set for the first implementation slice is defined in [starter-test-matrix.md](./starter-test-matrix.md).
Concrete starter fixtures live under [fixtures](../fixtures).

## Round-trip cases

- Single-file model with one package and a few parts
- Multi-file model with imports and cross-file references
- Rename a part and confirm references update correctly
- Add a port and connection, then verify the saved text
- Add a requirement and satisfy edge, then verify traceability output
- Delete an element and confirm all dependent edges are handled safely

## Negative cases

- Unsupported SysML syntax appears in an input file
- A file is partially written or malformed
- A relationship points to a missing target
- Two elements share a display name but not an identity
- A save operation would rewrite unrelated content
