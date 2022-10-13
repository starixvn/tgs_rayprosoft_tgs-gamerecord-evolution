#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BuildConfiguration=Release
RUN --mount=type=secret,id=nuget,dst=/pat cp /pat ~/.nuget/NuGet/NuGet.Config
WORKDIR /src
COPY ["NuGet.config", "."]
COPY ["Tgs.GameRecord.EvolutionManualService/Tgs.GameRecord.EvolutionManualService.csproj", "Tgs.GameRecord.EvolutionManualService/"]
COPY ["Tgs.GameRecord.EvolutionService.Core/Tgs.GameRecord.EvolutionService.Core.csproj", "Tgs.GameRecord.EvolutionService.Core/"]
RUN dotnet restore "Tgs.GameRecord.EvolutionManualService/Tgs.GameRecord.EvolutionManualService.csproj"
COPY . .
WORKDIR "/src/Tgs.GameRecord.EvolutionManualService"
RUN dotnet build "Tgs.GameRecord.EvolutionManualService.csproj" -c $BuildConfiguration -o /app/build

FROM build AS publish
ARG BuildConfiguration
RUN dotnet publish "Tgs.GameRecord.EvolutionManualService.csproj" -c $BuildConfiguration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.GameRecord.EvolutionManualService.dll"]