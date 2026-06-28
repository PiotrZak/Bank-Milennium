using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Cards.Application.Models;
using Cards.Domain.Enums;
using Cards.Domain.Rules;
using Cards.Domain.Samples;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace Cards.Api.Tests;

public sealed class CardsApiTests : IClassFixture<FastCardsWebApplicationFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly HttpClient _client;

    public CardsApiTests(FastCardsWebApplicationFactory factory) => _client = factory.CreateClient();

    [Fact]
    public async Task GetAllowedActions_returns_200_for_valid_card()
    {
        var response = await _client.GetAsync(
            $"/users/User1/cards/{SampleCardPans.User1Card17UriEncoded}/allowed-actions");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<GetCardAllowedActionsResult>(JsonOptions);
        body!.UserId.Should().Be("User1");
        body.CardNumber.Should().Be(SampleCardPans.User1Card17);
        body.CardType.Should().Be(CardType.Prepaid);
        body.CardStatus.Should().Be(CardStatus.Closed);
        body.AllowedActions.Should().Equal(
            CardAllowedActionsCatalog.GetAllowedActions(CardType.Prepaid, CardStatus.Closed, false));
    }

    [Fact]
    public async Task GetAllowedActions_returns_camelCase_json()
    {
        var json = await _client.GetStringAsync(
            $"/users/User1/cards/{SampleCardPans.User1Card17UriEncoded}/allowed-actions");
        using var doc = JsonDocument.Parse(json);
        doc.RootElement.TryGetProperty("userId", out _).Should().BeTrue();
        doc.RootElement.TryGetProperty("allowedActions", out _).Should().BeTrue();
    }

    [Fact]
    public async Task GetAllowedActions_returns_404_for_unknown_card()
    {
        var response = await _client.GetAsync(
            $"/users/User1/cards/{SampleCardPans.User1Card99UriEncoded}/allowed-actions");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        await AssertProblemAsync(response, $"Card '{SampleCardPans.User1Card99}' not found for user 'User1'.");
    }

    [Fact]
    public async Task GetAllowedActions_returns_400_for_whitespace_userId()
    {
        var response = await _client.GetAsync(
            $"/users/%20/cards/{SampleCardPans.User1Card17UriEncoded}/allowed-actions");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        await AssertProblemAsync(response, "userId must not be empty.");
    }

    [Fact]
    public async Task GetAllowedActions_returns_400_for_invalid_pan()
    {
        var response = await _client.GetAsync("/users/User1/cards/NOT-A-VALID-PAN/allowed-actions");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        await AssertProblemAsync(
            response,
            $"cardNumber must be a valid PAN (13–19 digits, Luhn check), e.g. {SampleCardPans.User1Card17}.");
    }

    [Trait("Category", "Smoke")]
    [Fact]
    public async Task Health_ready_returns_200() =>
        (await _client.GetAsync("/health/ready")).StatusCode.Should().Be(HttpStatusCode.OK);

    private static async Task AssertProblemAsync(HttpResponseMessage response, string expectedDetail)
    {
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/problem+json");
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problem!.Detail.Should().Be(expectedDetail);
    }
}
