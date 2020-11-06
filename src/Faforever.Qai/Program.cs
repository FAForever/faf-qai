using System;
using System.Net.Http;
using Faforever.Qai.Core;
using Faforever.Qai.Core.Operations.Player;
using Faforever.Qai.Core.Services;
using Faforever.Qai.Discord;
using Faforever.Qai.Discord.Core.Structures.Configurations;
using Faforever.Qai.Irc;

using IrcDotNet;

using McMaster.Extensions.CommandLineUtils;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Faforever.Qai
{
	public static class Program
	{
		public static int Main(string[] args)
		{
			CommandLineApplication app = new CommandLineApplication();

			app.HelpOption("-h|--help");

			app.OnExecuteAsync(async cancellationToken =>
			{
				ServiceCollection services = new ServiceCollection();
				services.AddLogging(options => options.AddConsole());
				services.AddSingleton<IFetchPlayerStatsOperation, ApiFetchPlayerStatsOperation>();
				services.AddSingleton<IPlayerService, OperationsPlayerService>();
				services.AddTransient<ICommandParser>(x => new SimpleCommandParser("!", x.GetService<IPlayerService>()));
				services.AddTransient<HttpClient>();

				await using var serviceProvider = services.BuildServiceProvider();

				using QaIrc ircBot = new QaIrc("irc.faforever.com", new IrcUserRegistrationInfo
				{
					NickName = "Balleby",
					RealName = "Balleby",
					Password = "balleby",
					UserName = "balleby"
				}, serviceProvider.GetService<ILogger<QaIrc>>(), serviceProvider.GetService<ICommandParser>());
				ircBot.Run();

				Console.WriteLine(
					"Input Bot Token [FOR DEBUG ONLY - REMOVE IN PROD (or once we have a desicion on how to retrive config values)]: ");
				try
				{
					await using DiscordBot discordBot = new DiscordBot(serviceProvider, LogLevel.Debug,
						new DiscordBotConfiguration()
						{
							Token = Console.ReadLine(),
							Prefix = "!",
							Shards = 1
						});

					await discordBot.InitializeAsync();
					await discordBot.StartAsync();
				}
				catch (InvalidOperationException e)
				{
					serviceProvider.GetService<ILogger<DiscordBot>>().LogCritical(e.Message);
				}

				//TODO Exiting on Enter is probably ill advised
				Console.ReadLine();
			});

			return app.Execute(args);
		}
	}
}