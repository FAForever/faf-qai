FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /build

COPY ./Faforever.Qai.sln ./NuGet.config  ./

# First copy all csproj, a bit ugly. Is there a better way? 
COPY ./src/Faforever.Qai/Faforever.Qai.csproj ./src/Faforever.Qai/Faforever.Qai.csproj
COPY ./src/Faforever.Qai.Core/Faforever.Qai.Core.csproj ./src/Faforever.Qai.Core/Faforever.Qai.Core.csproj
COPY ./src/Faforever.Qai.Database.Setup/Faforever.Qai.Database.Setup.csproj ./src/Faforever.Qai.Database.Setup/Faforever.Qai.Database.Setup.csproj
COPY ./src/Faforever.Qai.Discord/Faforever.Qai.Discord.csproj ./src/Faforever.Qai.Discord/Faforever.Qai.Discord.csproj
COPY ./src/Faforever.Qai.Irc/Faforever.Qai.Irc.csproj ./src/Faforever.Qai.Irc/Faforever.Qai.Irc.csproj

COPY ./tests/Faforever.Qai.Core.Tests/Faforever.Qai.Core.Tests.csproj ./tests/Faforever.Qai.Core.Tests/Faforever.Qai.Core.Tests.csproj

# Install all necessary packages
RUN dotnet restore

COPY ./src ./src
COPY ./tests ./tests

RUN dotnet publish -c Release -o out
#COPY ./src/Faforever.Qai/Database out/Database

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app 
COPY --from=build /build/out ./
ENTRYPOINT ["dotnet", "Faforever.Qai.dll"]