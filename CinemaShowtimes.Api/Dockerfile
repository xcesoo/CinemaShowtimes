FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["CinemaShowtimes.Api/CinemaShowtimes.Api.csproj", "CinemaShowtimes.Api/"]
COPY ["CinemaShowtimes.Application/CinemaShowtimes.Application.csproj", "CinemaShowtimes.Application/"]
COPY ["CinemaShowtimes.Domain/CinemaShowtimes.Domain.csproj", "CinemaShowtimes.Domain/"]
COPY ["CinemaShowtimes.Infrastructure/CinemaShowtimes.Infrastructure.csproj", "CinemaShowtimes.Infrastructure/"]
RUN dotnet restore "CinemaShowtimes.Api/CinemaShowtimes.Api.csproj"

COPY . .
WORKDIR "/src/CinemaShowtimes.Api"
RUN dotnet build "CinemaShowtimes.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "CinemaShowtimes.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CinemaShowtimes.Api.dll"]