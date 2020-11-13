FROM mcr.microsoft.com/dotnet/aspnet:5.0

COPY src/Faforever.Qai/bin/Release/net5.0/publish/ App/
WORKDIR /App
ENTRYPOINT ["dotnet", "Faforever.Qai.dll"]