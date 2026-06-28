using Cards.Application.Abstractions;
using Cards.Application.Exceptions;
using Cards.Application.Services;
using Cards.Domain.Entities;
using Cards.Domain.Enums;
using Cards.Domain.Rules;
using Cards.Domain.Samples;
using FluentAssertions;
using NSubstitute;

namespace Cards.Application.Tests;

public sealed class GetCardAllowedActionsHandlerTests
{
    private readonly ICardRepository _cards = Substitute.For<ICardRepository>();
    private readonly GetCardAllowedActionsHandler _sut;

    public GetCardAllowedActionsHandlerTests() => _sut = new(_cards);

    [Fact]
    public async Task HandleAsync_returns_allowed_actions_when_card_exists()
    {
        var card = new CardDetails(SampleCardPans.User1Card17, CardType.Prepaid, CardStatus.Closed, false);
        _cards.GetCardAsync("User1", SampleCardPans.User1Card17, Arg.Any<CancellationToken>())
            .Returns(card);

        var result = await _sut.HandleAsync("User1", SampleCardPans.User1Card17);

        result.CardType.Should().Be(CardType.Prepaid);
        result.AllowedActions.Should().Equal(
            CardAllowedActionsCatalog.GetAllowedActions(CardType.Prepaid, CardStatus.Closed, false));
    }

    [Fact]
    public async Task HandleAsync_throws_when_card_not_found()
    {
        _cards.GetCardAsync("User1", SampleCardPans.User1Card99, Arg.Any<CancellationToken>())
            .Returns((CardDetails?)null);

        var act = () => _sut.HandleAsync("User1", SampleCardPans.User1Card99);

        await act.Should().ThrowAsync<CardNotFoundException>()
            .WithMessage($"Card '{SampleCardPans.User1Card99}' not found for user 'User1'.");
    }

    [Fact]
    public async Task HandleAsync_throws_when_userId_is_whitespace()
    {
        var act = () => _sut.HandleAsync(" ", SampleCardPans.User1Card17);
        await act.Should().ThrowAsync<InvalidCardRequestException>().WithMessage("userId must not be empty.");
        await _cards.DidNotReceiveWithAnyArgs().GetCardAsync(default!, default!, default);
    }

    [Fact]
    public async Task HandleAsync_throws_when_cardNumber_is_whitespace()
    {
        var act = () => _sut.HandleAsync("User1", " ");
        await act.Should().ThrowAsync<InvalidCardRequestException>().WithMessage("cardNumber must not be empty.");
        await _cards.DidNotReceiveWithAnyArgs().GetCardAsync(default!, default!, default);
    }

    [Fact]
    public async Task HandleAsync_throws_when_pan_is_invalid()
    {
        var act = () => _sut.HandleAsync("User1", "NOT-A-VALID-PAN");
        await act.Should().ThrowAsync<InvalidCardRequestException>()
            .WithMessage($"cardNumber must be a valid PAN (13–19 digits, Luhn check), e.g. {SampleCardPans.User1Card17}.");
    }
}
