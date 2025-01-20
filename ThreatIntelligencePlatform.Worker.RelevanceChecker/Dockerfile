FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ThreatIntelligencePlatform.Worker.RelevanceChecker/ThreatIntelligencePlatform.Worker.RelevanceChecker.csproj", "ThreatIntelligencePlatform.Worker.RelevanceChecker/"]
RUN dotnet restore "ThreatIntelligencePlatform.Worker.RelevanceChecker/ThreatIntelligencePlatform.Worker.RelevanceChecker.csproj"
COPY . .
WORKDIR "/src/ThreatIntelligencePlatform.Worker.RelevanceChecker"
RUN dotnet build "ThreatIntelligencePlatform.Worker.RelevanceChecker.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ThreatIntelligencePlatform.Worker.RelevanceChecker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ThreatIntelligencePlatform.Worker.RelevanceChecker.dll"]
