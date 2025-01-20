FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ThreatIntelligencePlatform.API/ThreatIntelligencePlatform.API.csproj", "ThreatIntelligencePlatform.API/"]
COPY ["ThreatIntelligencePlatform.Business/ThreatIntelligencePlatform.Business.csproj", "ThreatIntelligencePlatform.Business/"]
COPY ["ThreatIntelligencePlatform.DataAccess/ThreatIntelligencePlatform.DataAccess.csproj", "ThreatIntelligencePlatform.DataAccess/"]
COPY ["ThreatIntelligencePlatform.Configuration/ThreatIntelligencePlatform.Configuration.csproj", "ThreatIntelligencePlatform.Configuration/"]
RUN dotnet restore "./ThreatIntelligencePlatform.API/ThreatIntelligencePlatform.API.csproj"
COPY . .
WORKDIR "/src/ThreatIntelligencePlatform.API"
RUN dotnet build "./ThreatIntelligencePlatform.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ThreatIntelligencePlatform.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ThreatIntelligencePlatform.API.dll"]