FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Clinica.Api.Rest/Clinica.Api.Rest.csproj", "src/Clinica.Api.Rest/"]
COPY ["src/Clinica.Application/Clinica.Application.csproj", "src/Clinica.Application/"]
COPY ["src/Clinica.Domain/Clinica.Domain.csproj", "src/Clinica.Domain/"]
COPY ["src/Clinica.Infrastructure/Clinica.Infrastructure.csproj", "src/Clinica.Infrastructure/"]
COPY ["src/Clinica.Contracts/Clinica.Contracts.csproj", "src/Clinica.Contracts/"]
RUN dotnet restore "src/Clinica.Api.Rest/Clinica.Api.Rest.csproj"
COPY . .
WORKDIR "/src/src/Clinica.Api.Rest"
RUN dotnet build "Clinica.Api.Rest.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Clinica.Api.Rest.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Clinica.Api.Rest.dll"]
