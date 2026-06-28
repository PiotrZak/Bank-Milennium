using Cards.Api.Controllers;
using Cards.Domain.Samples;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Cards.Api.Swagger;

internal sealed class AllowedActionsOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (context.MethodInfo.DeclaringType != typeof(CardsController)
            || context.MethodInfo.Name != nameof(CardsController.GetAllowedActions))
        {
            return;
        }

        operation.Summary = "Get allowed card actions";
        operation.Description = $"""
            Returns permitted operations (`ACTION1`–`ACTION13`) for a user's card.

            **Flow:** validate `userId` and `cardNumber` (PAN) → fetch card from **`CardService.GetCardDetails`** (~1 s) → resolve allowed actions from card type, status, and PIN.

            | Status | When |
            |--------|------|
            | 200 | Card found |
            | 400 | Invalid `userId` or `cardNumber` |
            | 404 | Card not found in CardService |

            **Example:** `GET /users/User1/cards/{SampleCardPans.User1Card17}/allowed-actions`
            """;

        if (operation.Parameters.Count >= 2)
        {
            operation.Parameters[0].Description = "Sample users: `User1`, `User2`, `User3`";
            operation.Parameters[1].Description = $"Card PAN (13–19 digits, Luhn), e.g. `{SampleCardPans.User1Card17}`";
        }
    }
}
