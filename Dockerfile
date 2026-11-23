# syntax=docker/dockerfile:1

# ---- Build stage ----------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore first so this layer is cached until a project file changes.
COPY global.json Directory.Build.props ./
COPY src/ClaimGuard.Api/ClaimGuard.Api.csproj src/ClaimGuard.Api/
RUN dotnet restore src/ClaimGuard.Api/ClaimGuard.Api.csproj

COPY .editorconfig ./
COPY src/ src/
RUN dotnet publish src/ClaimGuard.Api/ClaimGuard.Api.csproj \
    -c Release -o /app/publish --no-restore /p:UseAppHost=false

# ---- Runtime stage --------------------------------------------------------
# Chiseled image: no shell, no package manager, runs as a non-root user by default.
# Smaller attack surface and fewer findings for Trivy.
FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble-chiseled AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080 \
    DOTNET_EnableDiagnostics=0
EXPOSE 8080
USER $APP_UID

ENTRYPOINT ["dotnet", "ClaimGuard.Api.dll"]
