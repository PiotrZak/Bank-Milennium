namespace Cards.Application.Exceptions;

public sealed class InvalidCardRequestException(string message) : Exception(message);
