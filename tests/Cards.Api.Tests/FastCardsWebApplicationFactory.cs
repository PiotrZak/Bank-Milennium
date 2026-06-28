using Cards.Application.Abstractions;
using Cards.Domain.Entities;
using Cards.Domain.Enums;
using Cards.Domain.Samples;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Cards.Api.Tests;

public sealed class FastCardsWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder) =>
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<ICardRepository>();
            services.AddSingleton<ICardRepository, FakeCardRepository>();
        });

    private sealed class FakeCardRepository : ICardRepository
    {
        private static readonly Dictionary<(string, string), CardDetails> Cards = new()
        {
            [("User1", SampleCardPans.User1Card17)] =
                new(SampleCardPans.User1Card17, CardType.Prepaid, CardStatus.Closed, false),
            [("User1", SampleCardPans.User1Card119)] =
                new(SampleCardPans.User1Card119, CardType.Credit, CardStatus.Blocked, false),
        };

        public Task<CardDetails?> GetCardAsync(string userId, string cardNumber, CancellationToken cancellationToken = default) =>
            Task.FromResult(Cards.TryGetValue((userId, cardNumber), out var card) ? card : null);
    }
}
