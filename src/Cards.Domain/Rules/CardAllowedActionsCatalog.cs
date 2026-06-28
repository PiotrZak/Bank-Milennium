using Cards.Domain.Enums;

namespace Cards.Domain.Rules;

/// <summary>
/// Precomputes allowed actions for every (card type, status, PIN) combination.
/// Rules live in <see cref="CardActionPermissionRules"/>; this class answers lookups at runtime.
/// </summary>
public static class CardAllowedActionsCatalog
{
    private static readonly IReadOnlyDictionary<CardState, string[]> Lookup = BuildLookup();

    public static IReadOnlyList<string> GetAllowedActions(CardType type, CardStatus status, bool isPinSet) =>
        Lookup[new CardState(type, status, isPinSet)];

    public static IEnumerable<object[]> AllStateCases()
    {
        foreach (var state in AllStates)
            yield return [state.Type, state.Status, state.IsPinSet];
    }

    private static IEnumerable<CardState> AllStates
    {
        get
        {
            foreach (CardType type in Enum.GetValues<CardType>())
            foreach (CardStatus status in Enum.GetValues<CardStatus>())
            foreach (var pin in new[] { false, true })
                yield return new CardState(type, status, pin);
        }
    }

    private static Dictionary<CardState, string[]> BuildLookup()
    {
        var lookup = new Dictionary<CardState, string[]>();
        foreach (var state in AllStates)
            lookup[state] = ComputeAllowedActions(state);
        return lookup;
    }

    private static string[] ComputeAllowedActions(CardState state)
    {
        var allowed = new List<string>(CardActionPermissionRules.All.Count);
        foreach (var rule in CardActionPermissionRules.All)
        {
            if (IsAllowed(rule, state))
                allowed.Add(rule.Operation);
        }
        return allowed.ToArray();
    }

    private static bool IsAllowed(CardActionPermissionRule rule, CardState state) =>
        rule.AllowedTypes.Contains(state.Type)
        && rule.StatusRules.TryGetValue(state.Status, out var eligibility)
        && MatchesPin(eligibility, state.IsPinSet);

    private static bool MatchesPin(StatusEligibility eligibility, bool isPinSet) =>
        eligibility switch
        {
            StatusEligibility.Always => true,
            StatusEligibility.Never => false,
            StatusEligibility.PinRequired => isPinSet,
            StatusEligibility.PinNotSet => !isPinSet,
            _ => false
        };

    private readonly record struct CardState(CardType Type, CardStatus Status, bool IsPinSet);
}
