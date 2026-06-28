using Cards.Domain.Enums;

namespace Cards.Application.Models;

public sealed record GetCardAllowedActionsResult(
    string UserId,
    string CardNumber,
    CardType CardType,
    CardStatus CardStatus,
    bool IsPinSet,
    IReadOnlyList<string> AllowedActions);
