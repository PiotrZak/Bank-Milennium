using Cards.Domain.Enums;

namespace Cards.Domain.Entities;

public record CardDetails(
    string CardNumber,
    CardType CardType,
    CardStatus CardStatus,
    bool IsPinSet);
