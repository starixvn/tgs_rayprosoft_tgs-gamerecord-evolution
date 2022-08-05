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
COPY ["Tgs.GameRecord.EvolutionService/Tgs.GameRecord.EvolutionService.csproj", "Tgs.GameRecord.EvolutionService/"]
COPY ["Tgs.GameRecord.EvolutionService.Core/Tgs.GameRecord.EvolutionService.Core.csproj", "Tgs.GameRecord.EvolutionService.Core/"]
RUN dotnet restore "Tgs.GameRecord.EvolutionService/Tgs.GameRecord.EvolutionService.csproj"
COPY . .
WORKDIR "/src/Tgs.GameRecord.EvolutionService"
RUN dotnet build "Tgs.GameRecord.EvolutionService.csproj" -c $BuildConfiguration -o /app/build

FROM build AS publish
ARG BuildConfiguration
RUN dotnet publish "Tgs.GameRecord.EvolutionService.csproj" -c $BuildConfiguration -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.GameRecord.EvolutionService.dll"]