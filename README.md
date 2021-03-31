![Continuous integration](https://github.com/FAForever/faf-qai/workflows/Continuous%20integration/badge.svg)
# Faforever.Qai
The rewrite and combining of Dostya/Qai into a single application.

## QAI
This is a total rewrite of [QAI](https://github.com/FAForever/QAI)

## Dostya
This is a total rewrite of [Dostya](https://github.com/FAForever/Dostya)

## Installation
Just dump the release in a folder and run the "Qai.exe" executable.

## Configuration
To Create a new Database file for testing use `Update-Database --project Faforever.Qai.Core` in the Packet Manager Console.
If you are using the .NET Core CLI, run `dotnet ef database update --project src/Faforever.Qai.Core` from the Faforever.Qai solution folder.

Once you have a test.db file, move it into the Faforever.Qai project if it is not there already, and make sure to set property `Copy to Output Directory` to Copy if newer. You can use Copy Always, but data will be reset between tests.

See [this reference](https://docs.microsoft.com/en-us/ef/core/miscellaneous/cli/dotnet) for installing the EF Core tools for the .NET Core CLI 

## Commands
In order to invoke a command you just type it in #aeolus in either the client or discord or you can send a private message to the bot.
You prefix all commands with an **'!'**. For an example in order to invoke the player command you do.
`!player coolmcgrrr`

See [Issiue #9](https://github.com/FAForever/faf-qai/issues/9) for a full list of commands.
