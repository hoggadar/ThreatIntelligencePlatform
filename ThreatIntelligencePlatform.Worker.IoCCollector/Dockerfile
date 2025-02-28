FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ThreatIntelligencePlatform.Worker.IoCCollector/ThreatIntelligencePlatform.Worker.IoCCollector.csproj", "ThreatIntelligencePlatform.Worker.IoCCollector/"]
COPY ["ThreatIntelligencePlatform.MessageBroker/ThreatIntelligencePlatform.MessageBroker.csproj", "ThreatIntelligencePlatform.MessageBroker/"]
COPY ["ThreatIntelligencePlatform.Configuration/ThreatIntelligencePlatform.Configuration.csproj", "ThreatIntelligencePlatform.Configuration/"]
COPY ["ThreatIntelligencePlatform.Shared/ThreatIntelligencePlatform.Shared.csproj", "ThreatIntelligencePlatform.Shared/"]
RUN dotnet restore "ThreatIntelligencePlatform.Worker.IoCCollector/ThreatIntelligencePlatform.Worker.IoCCollector.csproj"
COPY . .
WORKDIR "/src/ThreatIntelligencePlatform.Worker.IoCCollector"
RUN dotnet build "ThreatIntelligencePlatform.Worker.IoCCollector.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ThreatIntelligencePlatform.Worker.IoCCollector.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ThreatIntelligencePlatform.Worker.IoCCollector.dll"]
