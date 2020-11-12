using System;
using System.Net.Http;

using Faforever.Qai.Core;
using Faforever.Qai.Core.Commands.Arguments;
using Faforever.Qai.Core.Commands.Context;
using Faforever.Qai.Core.Database;
using Faforever.Qai.Core.Operations.Player;
using Faforever.Qai.Core.Services;
using Faforever.Qai.Discord;
using Faforever.Qai.Discord.Core.Structures.Configurations;
using Faforever.Qai.Irc;

using IrcDotNet;

using McMaster.Extensions.CommandLineUtils;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Qmmands;

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

				services.AddLogging(options => options.AddConsole())
					.AddDbContext<QAIDatabaseModel>()
					.AddSingleton<IFetchPlayerStatsOperation, ApiFetchPlayerStatsOperation>()
					.AddSingleton<IFindPlayerOperation, ApiFindPlayerOperation>()
					.AddSingleton<IPlayerService, OperationPlayerService>()
					.AddTransient<HttpClient>()
					.AddTransient<RelayService>()
					.AddSingleton((x) =>
					{
						var options = new CommandService(new CommandServiceConfiguration()
						{
							// Additional configuration for the command service goes here.
							
						});

						// Command modules go here.
						options.AddModules(System.Reflection.Assembly.GetAssembly(typeof(CustomCommandContext)));
						// Argument converters go here.
						options.AddTypeParser(new DiscordChannelTypeConverter());
						return options;
					})
					.AddSingleton<QCommandsHandler>();


				await using var serviceProvider = services.BuildServiceProvider();

				using QaIrc ircBot = new QaIrc("irc.faforever.com", new IrcUserRegistrationInfo
				{
					NickName = "Ballebyte",
					RealName = "Ballebyte",
					Password = "ballebyte",
					UserName = "ballebyte"
				}, serviceProvider.GetService<ILogger<QaIrc>>(),
					serviceProvider.GetService<QCommandsHandler>(),
					serviceProvider.GetService<RelayService>(), serviceProvider);
				ircBot.Run();

				Console.WriteLine(
					"Input Bot Token [FOR DEBUG ONLY - REMOVE IN PROD (or once we have a desicion on how to retrive config values)]: ");

				await using DiscordBot discordBot = new DiscordBot(serviceProvider, LogLevel.Debug,
					new DiscordBotConfiguration()
					{
						Token = Console.ReadLine(),
						Prefix = "!",
						Shards = 1
					});

				try
				{
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