using Cards.Application.Abstractions;
using Cards.Domain.Entities;
using Cards.Domain.Enums;
using Cards.Domain.Samples;
using Cards.Domain.Validation;

namespace Cards.Infrastructure.Persistence;

public sealed class CardService : ICardRepository
{
    private readonly Dictionary<string, Dictionary<string, CardDetails>> _userCards = CreateSampleUserCards();

    public async Task<CardDetails?> GetCardAsync(
        string userId,
        string cardNumber,
        CancellationToken cancellationToken = default)
    {
        await Task.Delay(1000, cancellationToken);
        var pan = PanValidator.Normalize(cardNumber);
        return _userCards.TryGetValue(userId, out var cards) && cards.TryGetValue(pan, out var card)
            ? card
            : null;
    }

    private static Dictionary<string, Dictionary<string, CardDetails>> CreateSampleUserCards()
    {
        var userCards = new Dictionary<string, Dictionary<string, CardDetails>>();
        for (var i = 1; i <= 3; i++)
        {
            var cards = new Dictionary<string, CardDetails>();
            var cardIndex = 1;
            foreach (CardType cardType in Enum.GetValues<CardType>())
            foreach (CardStatus cardStatus in Enum.GetValues<CardStatus>())
            {
                var cardNumber = SampleCardPans.Create(i, cardIndex);
                cards[cardNumber] = new CardDetails(cardNumber, cardType, cardStatus, cardIndex % 2 == 0);
                cardIndex++;
            }
            userCards[$"User{i}"] = cards;
        }
        return userCards;
    }
}
