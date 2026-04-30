# Structure Notes

This foundation uses the rule:

> One important public class per file.

Acceptable exceptions:

- tiny enums may be grouped only when very tightly related
- private helper classes
- generated DTOs

Current organization is intended for:

- easier IDE navigation
- cleaner Git diffs
- less painful Unity project growth
- easier refactor and tests
