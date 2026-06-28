using Cards.Application.Models;
using Cards.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cards.Api.Controllers;

[ApiController]
[Route("users/{userId}/cards")]
public sealed class CardsController(GetCardAllowedActionsHandler handler) : ControllerBase
{
    [HttpGet("{cardNumber}/allowed-actions")]
    public Task<GetCardAllowedActionsResult> GetAllowedActions(CancellationToken cancellationToken)
    {
        var userId = RouteData.Values["userId"]?.ToString() ?? string.Empty;
        var cardNumber = RouteData.Values["cardNumber"]?.ToString() ?? string.Empty;
        return handler.HandleAsync(userId, cardNumber, cancellationToken);
    }
}
