FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY Cards.sln ./
COPY src/ ./src/
RUN dotnet restore src/Cards.Api/Cards.Api.csproj
RUN dotnet publish src/Cards.Api/Cards.Api.csproj \
    -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 \
  CMD curl -f http://localhost:8080/health/ready || exit 1
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Cards.Api.dll"]
