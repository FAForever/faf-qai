using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

using Faforever.Qai.Core;
using Faforever.Qai.Core.Commands.Arguments;
using Faforever.Qai.Core.Commands.Context;
using Faforever.Qai.Core.Database;
using Faforever.Qai.Core.Operations.Player;
using Faforever.Qai.Core.Services;
using Faforever.Qai.Core.Services.BotFun;
using Faforever.Qai.Core.Structures.Configurations;
using Faforever.Qai.Discord;
using Faforever.Qai.Discord.Core.Structures.Configurations;
using Faforever.Qai.Irc;

using IrcDotNet;

using McMaster.Extensions.CommandLineUtils;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

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

				DatabaseConfiguration dbConfig;
				using(FileStream fs = new(Path.Join("Config", "database_config.json"), FileMode.Open))
				{
					using StreamReader sr = new(fs);
					var json = await sr.ReadToEndAsync();
					dbConfig = JsonConvert.DeserializeObject<DatabaseConfiguration>(json);
				}

				BotFunConfiguration botFunConfig;
				using (FileStream fs = new(Path.Join("Config", "games_config.json"), FileMode.Open))
				{
					using StreamReader sr = new(fs);
					var json = await sr.ReadToEndAsync();
					botFunConfig = JsonConvert.DeserializeObject<BotFunConfiguration>(json);
				}

				services.AddLogging(options => options.AddConsole())
					.AddDbContext<QAIDatabaseModel>(options =>
					{
						options.UseSqlite(dbConfig.DataSource);
					})
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
					.AddSingleton<QCommandsHandler>()
					.AddSingleton<IBotFunService>(new BotFunService(botFunConfig));

				await using var serviceProvider = services.BuildServiceProvider();

				IrcConfiguration ircConfig;
				using(FileStream fs = new(Path.Join("Config", "irc_config.json"), FileMode.Open))
				{
					using StreamReader sr = new(fs);
					var json = await sr.ReadToEndAsync();
					ircConfig = JsonConvert.DeserializeObject<IrcConfiguration>(json);
				}

				using QaIrc ircBot = new QaIrc(ircConfig.Connection, new IrcUserRegistrationInfo
				{
					NickName = ircConfig.NickName,
					RealName = ircConfig.RealName,
					Password = ircConfig.Password,
					UserName = ircConfig.UserName
				}, serviceProvider.GetService<ILogger<QaIrc>>(),
					serviceProvider.GetService<QCommandsHandler>(),
					serviceProvider.GetService<RelayService>(), serviceProvider);
				ircBot.Run();

				DiscordBotConfiguration discordConfig;
				using(FileStream fs = new(Path.Join("Config", "discord_config.json"), FileMode.Open))
				{
					using StreamReader sr = new(fs);
					var json = await sr.ReadToEndAsync();
					discordConfig = JsonConvert.DeserializeObject<DiscordBotConfiguration>(json);
				}

				await using DiscordBot discordBot = new DiscordBot(serviceProvider, LogLevel.Debug, discordConfig);

				try
				{
					await discordBot.InitializeAsync();
					await discordBot.StartAsync();
				}
				catch (InvalidOperationException e)
				{
					serviceProvider.GetService<ILogger<DiscordBot>>().LogCritical(e.Message);
				}

				// Dont ever stop running this task.
				await Task.Delay(-1);
			});

			return app.Execute(args);
		}
	}
}