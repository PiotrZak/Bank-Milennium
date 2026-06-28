using Cards.Domain.Entities;

namespace Cards.Application.Abstractions;

public interface ICardRepository
{
    Task<CardDetails?> GetCardAsync(
        string userId,
        string cardNumber,
        CancellationToken cancellationToken = default);
}
