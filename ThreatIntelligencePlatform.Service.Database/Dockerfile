FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ThreatIntelligencePlatform.Service.Database/ThreatIntelligencePlatform.Service.Database.csproj", "ThreatIntelligencePlatform.Service.Database/"]
RUN dotnet restore "ThreatIntelligencePlatform.Service.Database/ThreatIntelligencePlatform.Service.Database.csproj"
COPY . .
WORKDIR "/src/ThreatIntelligencePlatform.Service.Database"
RUN dotnet build "ThreatIntelligencePlatform.Service.Database.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ThreatIntelligencePlatform.Service.Database.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ThreatIntelligencePlatform.Service.Database.dll"]
