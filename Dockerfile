# syntax=docker/dockerfile:1.7
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS restore
WORKDIR /src

COPY Directory.Build.props Directory.Packages.props BookyServer.sln ./
COPY src/BookyServer.Api/BookyServer.Api.csproj src/BookyServer.Api/
COPY src/BookyServer.Application/BookyServer.Application.csproj src/BookyServer.Application/
COPY src/BookyServer.Interfaces/BookyServer.Interfaces.csproj src/BookyServer.Interfaces/
COPY src/BookyServer.Infrastructure/BookyServer.Infrastructure.csproj src/BookyServer.Infrastructure/
COPY src/BookyServer.Domain/BookyServer.Domain.csproj src/BookyServer.Domain/
RUN --mount=type=cache,target=/root/.nuget/packages dotnet restore BookyServer.sln

FROM restore AS publish
COPY src/ src/
RUN --mount=type=cache,target=/root/.nuget/packages dotnet publish src/BookyServer.Api/BookyServer.Api.csproj \
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
