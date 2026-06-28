namespace Cards.Application.Exceptions;

public sealed class CardNotFoundException : Exception
{
    public CardNotFoundException(string userId, string cardNumber)
        : base($"Card '{cardNumber}' not found for user '{userId}'.")
    {
        UserId = userId;
        CardNumber = cardNumber;
    }

    public string UserId { get; }
    public string CardNumber { get; }
}
