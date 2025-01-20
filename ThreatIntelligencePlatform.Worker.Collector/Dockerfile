FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ThreatIntelligencePlatform.Worker.Collector/ThreatIntelligencePlatform.Worker.Collector.csproj", "ThreatIntelligencePlatform.Worker.Collector/"]
COPY ["ThreatIntelligencePlatform.MessageBroker/ThreatIntelligencePlatform.MessageBroker.csproj", "ThreatIntelligencePlatform.MessageBroker/"]
COPY ["ThreatIntelligencePlatform.Configuration/ThreatIntelligencePlatform.Configuration.csproj", "ThreatIntelligencePlatform.Configuration/"]
COPY ["ThreatIntelligencePlatform.SharedData/ThreatIntelligencePlatform.SharedData.csproj", "ThreatIntelligencePlatform.SharedData/"]
RUN dotnet restore "ThreatIntelligencePlatform.Worker.Collector/ThreatIntelligencePlatform.Worker.Collector.csproj"
COPY . .
WORKDIR "/src/ThreatIntelligencePlatform.Worker.Collector"
RUN dotnet build "ThreatIntelligencePlatform.Worker.Collector.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ThreatIntelligencePlatform.Worker.Collector.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ThreatIntelligencePlatform.Worker.Collector.dll"]
