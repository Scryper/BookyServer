# syntax=docker/dockerfile:1.7
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS restore
WORKDIR /src

COPY Directory.Build.props Directory.Packages.props BookyServer.sln ./
COPY BookyServer.Api/BookyServer.Api.csproj BookyServer.Api/
COPY BookyServer.Application/BookyServer.Application.csproj BookyServer.Application/
COPY BookyServer.Interfaces/BookyServer.Interfaces.csproj BookyServer.Interfaces/
COPY BookyServer.Infrastructure/BookyServer.Infrastructure.csproj BookyServer.Infrastructure/
COPY BookyServer.Domain/BookyServer.Domain.csproj BookyServer.Domain/
RUN --mount=type=cache,target=/root/.nuget/packages dotnet restore BookyServer.sln

FROM restore AS publish
COPY . .
RUN --mount=type=cache,target=/root/.nuget/packages dotnet publish BookyServer.Api/BookyServer.Api.csproj \
    --configuration Release --no-restore --output /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble-chiseled-extra AS final
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8080 \
    DOTNET_EnableDiagnostics=0
EXPOSE 8080
COPY --from=publish --chown=$APP_UID:$APP_UID /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "BookyServer.Api.dll"]
