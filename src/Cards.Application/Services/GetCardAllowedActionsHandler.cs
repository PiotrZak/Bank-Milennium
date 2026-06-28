using Cards.Application.Abstractions;
using Cards.Application.Exceptions;
using Cards.Application.Models;
using Cards.Domain.Rules;
using Cards.Domain.Samples;
using Cards.Domain.Validation;

namespace Cards.Application.Services;

public sealed class GetCardAllowedActionsHandler(ICardRepository cards)
{
    public async Task<GetCardAllowedActionsResult> HandleAsync(
        string userId,
        string cardNumber,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new InvalidCardRequestException("userId must not be empty.");
        if (string.IsNullOrWhiteSpace(cardNumber))
            throw new InvalidCardRequestException("cardNumber must not be empty.");
        if (!PanValidator.IsValid(cardNumber))
        {
            throw new InvalidCardRequestException(
                $"cardNumber must be a valid PAN (13–19 digits, Luhn check), e.g. {SampleCardPans.User1Card17}.");
        }

        var pan = PanValidator.Normalize(cardNumber);
        var card = await cards.GetCardAsync(userId, pan, cancellationToken)
            ?? throw new CardNotFoundException(userId, pan);

        return new GetCardAllowedActionsResult(
            userId,
            card.CardNumber,
            card.CardType,
            card.CardStatus,
            card.IsPinSet,
            CardAllowedActionsCatalog.GetAllowedActions(card.CardType, card.CardStatus, card.IsPinSet));
    }
}
