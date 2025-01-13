FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ThreatIntelligencePlatform.Api/ThreatIntelligencePlatform.Api.csproj", "ThreatIntelligencePlatform.Api/"]
COPY ["ThreatIntelligencePlatform.Business/ThreatIntelligencePlatform.Business.csproj", "ThreatIntelligencePlatform.Business/"]
COPY ["ThreatIntelligencePlatform.DataAccess/ThreatIntelligencePlatform.DataAccess.csproj", "ThreatIntelligencePlatform.DataAccess/"]
COPY ["ThreatIntelligencePlatform.Configuration/ThreatIntelligencePlatform.Configuration.csproj", "ThreatIntelligencePlatform.Configuration/"]
RUN dotnet restore "./ThreatIntelligencePlatform.Api/ThreatIntelligencePlatform.Api.csproj"
COPY . .
WORKDIR "/src/ThreatIntelligencePlatform.Api"
RUN dotnet build "./ThreatIntelligencePlatform.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ThreatIntelligencePlatform.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ThreatIntelligencePlatform.Api.dll"]