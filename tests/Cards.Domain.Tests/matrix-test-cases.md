# Permission matrix test cases (42)

Source: `docs/allowed-actions-permission-matrix.csv`  
Test: `CardAllowedActionsCatalogTests.GetAllowedActions_matches_permission_matrix_csv`  
Expected: `PermissionMatrixCsv` | Actual: `CardAllowedActionsCatalog`

| # | Type | Status | PIN | Expected actions |
|---|------|--------|-----|------------------|
| 1 | Prepaid | Ordered | false | ACTION3, ACTION4, ACTION7, ACTION8, ACTION9, ACTION10, ACTION12, ACTION13 |
| 2 | Prepaid | Ordered | true | ACTION3, ACTION4, ACTION6, ACTION8, ACTION9, ACTION10, ACTION12, ACTION13 |
| 3 | Prepaid | Inactive | false | ACTION2, ACTION3, ACTION4, ACTION7, ACTION8, ACTION9, ACTION10, ACTION11, ACTION12, ACTION13 |
| 4 | Prepaid | Inactive | true | ACTION2, ACTION3, ACTION4, ACTION6, ACTION8, ACTION9, ACTION10, ACTION11, ACTION12, ACTION13 |
| 5 | Prepaid | Active | false | ACTION1, ACTION3, ACTION4, ACTION7, ACTION8, ACTION9, ACTION10, ACTION11, ACTION12, ACTION13 |
| 6 | Prepaid | Active | true | ACTION1, ACTION3, ACTION4, ACTION6, ACTION8, ACTION9, ACTION10, ACTION11, ACTION12, ACTION13 |
| 7 | Prepaid | Restricted | false | ACTION3, ACTION4, ACTION9 |
| 8 | Prepaid | Restricted | true | ACTION3, ACTION4, ACTION9 |
| 9 | Prepaid | Blocked | false | ACTION3, ACTION4, ACTION8, ACTION9 |
| 10 | Prepaid | Blocked | true | ACTION3, ACTION4, ACTION6, ACTION7, ACTION8, ACTION9 |
| 11 | Prepaid | Expired | false | ACTION3, ACTION4, ACTION9 |
| 12 | Prepaid | Expired | true | ACTION3, ACTION4, ACTION9 |
| 13 | Prepaid | Closed | false | ACTION3, ACTION4, ACTION9 |
| 14 | Prepaid | Closed | true | ACTION3, ACTION4, ACTION9 |
| 15 | Debit | Ordered | false | ACTION3, ACTION4, ACTION7, ACTION8, ACTION9, ACTION10, ACTION12, ACTION13 |
| 16 | Debit | Ordered | true | ACTION3, ACTION4, ACTION6, ACTION8, ACTION9, ACTION10, ACTION12, ACTION13 |
| 17 | Debit | Inactive | false | ACTION2, ACTION3, ACTION4, ACTION7, ACTION8, ACTION9, ACTION10, ACTION11, ACTION12, ACTION13 |
| 18 | Debit | Inactive | true | ACTION2, ACTION3, ACTION4, ACTION6, ACTION8, ACTION9, ACTION10, ACTION11, ACTION12, ACTION13 |
| 19 | Debit | Active | false | ACTION1, ACTION3, ACTION4, ACTION7, ACTION8, ACTION9, ACTION10, ACTION11, ACTION12, ACTION13 |
| 20 | Debit | Active | true | ACTION1, ACTION3, ACTION4, ACTION6, ACTION8, ACTION9, ACTION10, ACTION11, ACTION12, ACTION13 |
| 21 | Debit | Restricted | false | ACTION3, ACTION4, ACTION9 |
| 22 | Debit | Restricted | true | ACTION3, ACTION4, ACTION9 |
| 23 | Debit | Blocked | false | ACTION3, ACTION4, ACTION8, ACTION9 |
| 24 | Debit | Blocked | true | ACTION3, ACTION4, ACTION6, ACTION7, ACTION8, ACTION9 |
| 25 | Debit | Expired | false | ACTION3, ACTION4, ACTION9 |
| 26 | Debit | Expired | true | ACTION3, ACTION4, ACTION9 |
| 27 | Debit | Closed | false | ACTION3, ACTION4, ACTION9 |
| 28 | Debit | Closed | true | ACTION3, ACTION4, ACTION9 |
| 29 | Credit | Ordered | false | ACTION3, ACTION4, ACTION5, ACTION7, ACTION8, ACTION9, ACTION10, ACTION12, ACTION13 |
| 30 | Credit | Ordered | true | ACTION3, ACTION4, ACTION5, ACTION6, ACTION8, ACTION9, ACTION10, ACTION12, ACTION13 |
| 31 | Credit | Inactive | false | ACTION2, ACTION3, ACTION4, ACTION5, ACTION7, ACTION8, ACTION9, ACTION10, ACTION11, ACTION12, ACTION13 |
| 32 | Credit | Inactive | true | ACTION2, ACTION3, ACTION4, ACTION5, ACTION6, ACTION8, ACTION9, ACTION10, ACTION11, ACTION12, ACTION13 |
| 33 | Credit | Active | false | ACTION1, ACTION3, ACTION4, ACTION5, ACTION7, ACTION8, ACTION9, ACTION10, ACTION11, ACTION12, ACTION13 |
| 34 | Credit | Active | true | ACTION1, ACTION3, ACTION4, ACTION5, ACTION6, ACTION8, ACTION9, ACTION10, ACTION11, ACTION12, ACTION13 |
| 35 | Credit | Restricted | false | ACTION3, ACTION4, ACTION5, ACTION9 |
| 36 | Credit | Restricted | true | ACTION3, ACTION4, ACTION5, ACTION9 |
| 37 | Credit | Blocked | false | ACTION3, ACTION4, ACTION5, ACTION8, ACTION9 |
| 38 | Credit | Blocked | true | ACTION3, ACTION4, ACTION5, ACTION6, ACTION7, ACTION8, ACTION9 |
| 39 | Credit | Expired | false | ACTION3, ACTION4, ACTION5, ACTION9 |
| 40 | Credit | Expired | true | ACTION3, ACTION4, ACTION5, ACTION9 |
| 41 | Credit | Closed | false | ACTION3, ACTION4, ACTION5, ACTION9 |
| 42 | Credit | Closed | true | ACTION3, ACTION4, ACTION5, ACTION9 |

## Spot-checks used elsewhere

| Case | PAN (User1) | Used in |
|------|-------------|---------|
| #13 Prepaid, Closed, PIN false | `4010000000000174` | API / smoke tests |
| #37 Credit, Blocked, PIN false | `4010000000001198` | FakeCardRepository |
