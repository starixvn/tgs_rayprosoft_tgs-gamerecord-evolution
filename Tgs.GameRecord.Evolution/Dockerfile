#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Tgs.GameRecord.Evolution/Tgs.GameRecord.Evolution.csproj", "Tgs.GameRecord.Evolution/"]
COPY ["Tgs.GameRecord.Evolution.Common/Tgs.GameRecord.Evolution.Common.csproj", "Tgs.GameRecord.Evolution.Common/"]
COPY ["Tgs.GameRecord.Evolution.Core/Tgs.GameRecord.Evolution.Core.csproj", "Tgs.GameRecord.Evolution.Core/"]
COPY ["Tgs.GameRecord.Evolution.Repositories/Tgs.GameRecord.Evolution.Repositories.csproj", "Tgs.GameRecord.Evolution.Repositories/"]
RUN dotnet restore "./Tgs.GameRecord.Evolution/Tgs.GameRecord.Evolution.csproj"
COPY . .
WORKDIR "/src/Tgs.GameRecord.Evolution"
RUN dotnet build "./Tgs.GameRecord.Evolution.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Tgs.GameRecord.Evolution.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.GameRecord.Evolution.dll"]