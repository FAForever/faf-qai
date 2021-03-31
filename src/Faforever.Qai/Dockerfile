#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["NuGet.config", "."]
COPY ["Faforever.Qai/Faforever.Qai.csproj", "Faforever.Qai/"]
RUN dotnet restore "Faforever.Qai/Faforever.Qai.csproj"
COPY . .
WORKDIR "/src/Faforever.Qai"
RUN dotnet build "Faforever.Qai.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Faforever.Qai.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Faforever.Qai.dll"]