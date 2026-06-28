using Cards.Domain.Enums;

namespace Cards.Domain.Rules;

public static class CardActionPermissionRules
{
    private static readonly CardType[] AllTypes = [CardType.Prepaid, CardType.Debit, CardType.Credit];
    private static readonly CardType[] CreditOnly = [CardType.Credit];

    public static IReadOnlyList<CardActionPermissionRule> All { get; } =
    [
        Rule("ACTION1", AllTypes, ActiveOnly()),
        Rule("ACTION2", AllTypes, InactiveOnly()),
        Rule("ACTION3", AllTypes, AllAlways()),
        Rule("ACTION4", AllTypes, AllAlways()),
        Rule("ACTION5", CreditOnly, AllAlways()),
        Rule("ACTION6", AllTypes, PinRequiredFor(CardStatus.Ordered, CardStatus.Inactive, CardStatus.Active, CardStatus.Blocked)),
        Rule("ACTION7", AllTypes, MixedAction7()),
        Rule("ACTION8", AllTypes, OrderedInactiveActiveBlocked()),
        Rule("ACTION9", AllTypes, AllAlways()),
        Rule("ACTION10", AllTypes, OrderedInactiveActive()),
        Rule("ACTION11", AllTypes, InactiveActive()),
        Rule("ACTION12", AllTypes, OrderedInactiveActive()),
        Rule("ACTION13", AllTypes, OrderedInactiveActive()),
    ];

    private static CardActionPermissionRule Rule(
        string operation,
        CardType[] types,
        Dictionary<CardStatus, StatusEligibility> statuses) =>
        new(operation, new HashSet<CardType>(types), statuses);

    private static Dictionary<CardStatus, StatusEligibility> AllAlways() =>
        AllStatuses(StatusEligibility.Always);

    private static Dictionary<CardStatus, StatusEligibility> ActiveOnly() =>
        AllStatuses(StatusEligibility.Never, (CardStatus.Active, StatusEligibility.Always));

    private static Dictionary<CardStatus, StatusEligibility> InactiveOnly() =>
        AllStatuses(StatusEligibility.Never, (CardStatus.Inactive, StatusEligibility.Always));

    private static Dictionary<CardStatus, StatusEligibility> OrderedInactiveActive() =>
        AllStatuses(
            StatusEligibility.Never,
            (CardStatus.Ordered, StatusEligibility.Always),
            (CardStatus.Inactive, StatusEligibility.Always),
            (CardStatus.Active, StatusEligibility.Always));

    private static Dictionary<CardStatus, StatusEligibility> InactiveActive() =>
        AllStatuses(
            StatusEligibility.Never,
            (CardStatus.Inactive, StatusEligibility.Always),
            (CardStatus.Active, StatusEligibility.Always));

    private static Dictionary<CardStatus, StatusEligibility> OrderedInactiveActiveBlocked() =>
        AllStatuses(
            StatusEligibility.Never,
            (CardStatus.Ordered, StatusEligibility.Always),
            (CardStatus.Inactive, StatusEligibility.Always),
            (CardStatus.Active, StatusEligibility.Always),
            (CardStatus.Blocked, StatusEligibility.Always));

    private static Dictionary<CardStatus, StatusEligibility> PinRequiredFor(params CardStatus[] statuses)
    {
        var rules = AllStatuses(StatusEligibility.Never);
        foreach (var status in statuses)
            rules[status] = StatusEligibility.PinRequired;
        return rules;
    }

    private static Dictionary<CardStatus, StatusEligibility> MixedAction7() =>
        AllStatuses(
            StatusEligibility.Never,
            (CardStatus.Ordered, StatusEligibility.PinNotSet),
            (CardStatus.Inactive, StatusEligibility.PinNotSet),
            (CardStatus.Active, StatusEligibility.PinNotSet),
            (CardStatus.Blocked, StatusEligibility.PinRequired));

    private static Dictionary<CardStatus, StatusEligibility> AllStatuses(
        StatusEligibility defaultEligibility,
        params (CardStatus Status, StatusEligibility Eligibility)[] overrides)
    {
        var rules = Enum.GetValues<CardStatus>()
            .ToDictionary(s => s, _ => defaultEligibility);

        foreach (var (status, eligibility) in overrides)
            rules[status] = eligibility;

        return rules;
    }
}

public sealed record CardActionPermissionRule(
    string Operation,
    IReadOnlySet<CardType> AllowedTypes,
    IReadOnlyDictionary<CardStatus, StatusEligibility> StatusRules);
