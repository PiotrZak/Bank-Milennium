# Allowed Actions — Expected Results

Expected resolver output is defined once in code:

**`src/Cards.Domain/Rules/CardActionPermissionRules.cs`** → built into **`CardAllowedActionsCatalog`**

Tests iterate all **42** combinations `(CardType × CardStatus × IsPinSet)` and assert the catalog matches [allowed-actions-permission-matrix.csv](./allowed-actions-permission-matrix.csv) via `PermissionMatrixCsv` in `tests/Cards.Domain.Tests/Matrix/`.

```bash
dotnet test tests/Cards.Domain.Tests
```

Human-readable matrix (PDF source): [allowed-actions-permission-matrix.csv](./allowed-actions-permission-matrix.csv)

## PDF spot-check examples (User1)

| PAN | Card index | Type | Status | PIN | Expected actions |
|-----|------------|------|--------|-----|------------------|
| `4010000000000174` | 17 | Prepaid | Closed | false | ACTION3, ACTION4, ACTION9 |
| `4010000000001198` | 119 | Credit | Blocked | false | ACTION3, ACTION4, ACTION5, ACTION8, ACTION9 |

PANs are Luhn-valid sample numbers from `SamplePanFactory` / `SampleCardPans`.
