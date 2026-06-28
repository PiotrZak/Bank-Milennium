using Cards.Domain.Enums;
using Cards.Domain.Rules;
using Cards.Domain.Tests.Matrix;
using FluentAssertions;

namespace Cards.Domain.Tests;

public sealed class CardAllowedActionsCatalogTests
{
    [Theory]
    [MemberData(nameof(AllStates))]
    public void GetAllowedActions_matches_permission_matrix_csv(
        CardType type,
        CardStatus status,
        bool isPinSet)
    {
        var expected = PermissionMatrixCsv.GetExpectedActions(type, status, isPinSet);
        var actual = CardAllowedActionsCatalog.GetAllowedActions(type, status, isPinSet);

        actual.Should().Equal(expected, because:
            $"{type}, {status}, PIN={isPinSet} should match docs/allowed-actions-permission-matrix.csv");
    }

    public static IEnumerable<object[]> AllStates => PermissionMatrixCsv.AllStateCases();
}
