FROM mcr.microsoft.com/dotnet/runtime:8.0-alpine AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Xenon/Xenon.csproj", "Xenon/"]
RUN dotnet restore "Xenon/Xenon.csproj"
COPY . .
WORKDIR "/src/Xenon"
RUN dotnet build "Xenon.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
RUN dotnet publish "Xenon.csproj" -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Xenon.dll"]
