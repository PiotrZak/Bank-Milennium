using System.Text.Json.Serialization;
using Cards.Api.Middleware;
using Cards.Api.Swagger;
using Cards.Application.Abstractions;
using Cards.Application.Services;
using Cards.Infrastructure.Persistence;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.OperationFilter<AllowedActionsOperationFilter>();
    o.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Cards API",
        Version = "v1",
        Description = """
            Allowed card operations (`ACTION1`–`ACTION13`) for `/users/{userId}/cards/{cardNumber}/allowed-actions`.

            Card metadata comes from **`CardService.GetCardDetails`**. Permissions are resolved locally.

            **Users:** `User1`, `User2`, `User3` | **Health:** `/health`, `/health/live`, `/health/ready`
            """
    });
});
builder.Services.AddHealthChecks();
builder.Services.AddSingleton<CardService>();
builder.Services.AddSingleton<ICardRepository>(sp => sp.GetRequiredService<CardService>());
builder.Services.AddSingleton<GetCardAllowedActionsHandler>();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(o => o.SwaggerEndpoint("/swagger/v1/swagger.json", "Cards API v1"));
}

app.MapControllers();
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/live");
app.MapHealthChecks("/health/ready");

app.Run();

public partial class Program;
