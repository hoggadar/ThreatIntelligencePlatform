FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ThreatIntelligencePlatform.CollectorService/ThreatIntelligencePlatform.CollectorService.csproj", "ThreatIntelligencePlatform.CollectorService/"]
COPY ["ThreatIntelligencePlatform.SharedData/ThreatIntelligencePlatform.SharedData.csproj", "ThreatIntelligencePlatform.SharedData/"]
RUN dotnet restore "ThreatIntelligencePlatform.CollectorService/ThreatIntelligencePlatform.CollectorService.csproj"
COPY . .
WORKDIR "/src/ThreatIntelligencePlatform.CollectorService"
RUN dotnet build "ThreatIntelligencePlatform.CollectorService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ThreatIntelligencePlatform.CollectorService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ThreatIntelligencePlatform.CollectorService.dll"]
