FROM mcr.microsoft.com/dotnet/core/aspnet:3.1

COPY src/Faforever.Qai/bin/Release/netcoreapp3.1/publish/ App/
WORKDIR /App
ENTRYPOINT ["dotnet", "Faforever.Qai.dll"]