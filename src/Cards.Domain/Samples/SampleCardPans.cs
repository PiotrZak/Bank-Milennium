using Cards.Domain.Validation;

namespace Cards.Domain.Samples;

public static class SampleCardPans
{
    public static string Create(int userIndex, int cardIndex) =>
        PanValidator.WithCheckDigit($"4{userIndex:D2}{cardIndex:D12}");

    public static string User1Card17 { get; } = Create(1, 17);
    public static string User1Card119 { get; } = Create(1, 119);
    public static string User1Card99 { get; } = Create(1, 99);
    public static string User1Card17UriEncoded { get; } = Uri.EscapeDataString(User1Card17);
    public static string User1Card119UriEncoded { get; } = Uri.EscapeDataString(User1Card119);
    public static string User1Card99UriEncoded { get; } = Uri.EscapeDataString(User1Card99);
}
