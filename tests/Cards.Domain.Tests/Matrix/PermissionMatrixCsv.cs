using Cards.Domain.Enums;

namespace Cards.Domain.Tests.Matrix;

/// <summary>
/// Expected allowed actions derived from docs/allowed-actions-permission-matrix.csv.
/// </summary>
internal static class PermissionMatrixCsv
{
    private static readonly Lazy<IReadOnlyList<MatrixRow>> Rows = new(ParseRows);

    public static IReadOnlyList<string> GetExpectedActions(CardType type, CardStatus status, bool isPinSet)
    {
        var allowed = new List<string>();
        foreach (var row in Rows.Value)
        {
            if (!IsTypeAllowed(row, type))
                continue;

            if (MatchesPin(ParseStatusCell(row.StatusCells[status]), isPinSet))
                allowed.Add(row.Action);
        }

        return allowed;
    }

    public static IEnumerable<object[]> AllStateCases()
    {
        foreach (CardType type in Enum.GetValues<CardType>())
        foreach (CardStatus status in Enum.GetValues<CardStatus>())
        foreach (var pin in new[] { false, true })
            yield return [type, status, pin];
    }

    private static IReadOnlyList<MatrixRow> ParseRows()
    {
        var lines = File.ReadAllLines(GetCsvPath());
        var rows = new List<MatrixRow>();

        for (var i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i]))
                continue;

            var cells = lines[i].Split(',');
            rows.Add(new MatrixRow(
                cells[0].Trim(),
                cells[1].Trim(),
                cells[2].Trim(),
                cells[3].Trim(),
                new Dictionary<CardStatus, string>
                {
                    [CardStatus.Ordered] = cells[4].Trim(),
                    [CardStatus.Inactive] = cells[5].Trim(),
                    [CardStatus.Active] = cells[6].Trim(),
                    [CardStatus.Restricted] = cells[7].Trim(),
                    [CardStatus.Blocked] = cells[8].Trim(),
                    [CardStatus.Expired] = cells[9].Trim(),
                    [CardStatus.Closed] = cells[10].Trim(),
                }));
        }

        return rows;
    }

    private static string GetCsvPath()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            var candidate = Path.Combine(dir.FullName, "docs", "allowed-actions-permission-matrix.csv");
            if (File.Exists(candidate))
                return candidate;

            dir = dir.Parent;
        }

        throw new FileNotFoundException("allowed-actions-permission-matrix.csv not found");
    }

    private static bool IsTypeAllowed(MatrixRow row, CardType type) =>
        ParseTypeCell(type switch
        {
            CardType.Prepaid => row.Prepaid,
            CardType.Debit => row.Debit,
            CardType.Credit => row.Credit,
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        });

    private static bool ParseTypeCell(string cell) =>
        cell.StartsWith("TAK", StringComparison.OrdinalIgnoreCase);

    private static PinRule ParseStatusCell(string cell)
    {
        if (cell.StartsWith("NIE", StringComparison.OrdinalIgnoreCase))
            return PinRule.Never;

        if (cell.Contains("brak pin", StringComparison.OrdinalIgnoreCase))
            return PinRule.PinNotSet;

        if (cell.Contains("pin nadany", StringComparison.OrdinalIgnoreCase)
            || cell.Contains("jak nie ma pin to NIE", StringComparison.OrdinalIgnoreCase))
            return PinRule.PinRequired;

        if (cell.StartsWith("TAK", StringComparison.OrdinalIgnoreCase))
            return PinRule.Always;

        return PinRule.Never;
    }

    private static bool MatchesPin(PinRule rule, bool isPinSet) =>
        rule switch
        {
            PinRule.Always => true,
            PinRule.Never => false,
            PinRule.PinRequired => isPinSet,
            PinRule.PinNotSet => !isPinSet,
            _ => false
        };

    private sealed record MatrixRow(
        string Action,
        string Prepaid,
        string Debit,
        string Credit,
        IReadOnlyDictionary<CardStatus, string> StatusCells);

    private enum PinRule
    {
        Always,
        Never,
        PinRequired,
        PinNotSet
    }
}
