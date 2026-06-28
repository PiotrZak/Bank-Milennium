using Cards.Domain.Samples;
using Cards.Domain.Validation;
using FluentAssertions;

namespace Cards.Domain.Tests;

public sealed class PanValidatorTests
{
    [Theory]
    [InlineData("4111111111111111")]
    [InlineData("5500 0000 0000 0004")]
    [InlineData("378282246310005")]
    public void IsValid_accepts_known_valid_pans(string pan)
    {
        PanValidator.IsValid(pan).Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("123")]
    [InlineData("4111111111111112")]
    [InlineData("ABCD123456789012")]
    public void IsValid_rejects_invalid_pans(string pan)
    {
        PanValidator.IsValid(pan).Should().BeFalse();
    }

    [Fact]
    public void SampleCardPans_are_valid()
    {
        PanValidator.IsValid(SampleCardPans.User1Card17).Should().BeTrue();
        PanValidator.IsValid(SampleCardPans.Create(1, 17)).Should().BeTrue();
    }
}
