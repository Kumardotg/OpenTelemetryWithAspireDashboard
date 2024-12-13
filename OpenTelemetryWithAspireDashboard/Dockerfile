FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5138

ENV ASPNETCORE_URLS=http://+:5138

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["OpenTelemetryWithAspireDashboard/OpenTelemetryWithAspireDashboard.csproj", "OpenTelemetryWithAspireDashboard/"]
RUN dotnet restore "OpenTelemetryWithAspireDashboard/OpenTelemetryWithAspireDashboard.csproj"
COPY . .
WORKDIR "/src/OpenTelemetryWithAspireDashboard"
RUN dotnet build "OpenTelemetryWithAspireDashboard.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "OpenTelemetryWithAspireDashboard.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OpenTelemetryWithAspireDashboard.dll"]
