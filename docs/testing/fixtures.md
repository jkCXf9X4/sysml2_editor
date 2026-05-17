# Test Fixtures

The repo should maintain a small fixture library that grows with the product.

## Fixture rules

- Keep fixtures deterministic and small unless the test is explicitly performance-oriented
- Prefer checked-in fixtures over dynamically generated ones for regression tests
- Version any generated expected output alongside the fixture
- Use the same fixtures across unit, integration, and E2E tests when possible
