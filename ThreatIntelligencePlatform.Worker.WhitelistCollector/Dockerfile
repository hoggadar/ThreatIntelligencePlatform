FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ThreatIntelligencePlatform.Worker.WhitelistCollector/ThreatIntelligencePlatform.Worker.WhitelistCollector.csproj", "ThreatIntelligencePlatform.Worker.WhitelistCollector/"]
RUN dotnet restore "ThreatIntelligencePlatform.Worker.WhitelistCollector/ThreatIntelligencePlatform.Worker.WhitelistCollector.csproj"
COPY . .
WORKDIR "/src/ThreatIntelligencePlatform.Worker.WhitelistCollector"
RUN dotnet build "ThreatIntelligencePlatform.Worker.WhitelistCollector.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ThreatIntelligencePlatform.Worker.WhitelistCollector.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ThreatIntelligencePlatform.Worker.WhitelistCollector.dll"]
