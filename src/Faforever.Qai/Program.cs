using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using Faforever.Qai.Core;
using Faforever.Qai.Core.Commands.Arguments;
using Faforever.Qai.Core.Commands.Arguments.Converters;
using Faforever.Qai.Core.Commands.Context;
using Faforever.Qai.Core.Database;
using Faforever.Qai.Core.Operations;
using Faforever.Qai.Core.Operations.Clients;
using Faforever.Qai.Core.Operations.Maps;
using Faforever.Qai.Core.Operations.Player;
using Faforever.Qai.Core.Operations.Units;
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
		public static readonly Uri ApiUri = new Uri("https://api.faforever.com/");

		public static int Main(string[] args)
		{
			CommandLineApplication app = new CommandLineApplication();

			app.HelpOption("-h|--help");

			app.OnExecuteAsync(async cancellationToken =>
			{
				ServiceCollection services = new ServiceCollection();

				DatabaseConfiguration dbConfig = new()
				{
					DataSource = $"Data Source={Environment.GetEnvironmentVariable("DATA_SOURCE")}"
				};

#if DEBUG
				// For use when the DB is Database/db-name.db
				if (!Directory.Exists("Database"))
					Directory.CreateDirectory("Database");
#endif

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
					.AddSingleton<RelayService>()
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
						options.AddTypeParser(new BotUserCapsuleConverter());
						return options;
					})
					.AddSingleton<QCommandsHandler>()
					.AddSingleton<IBotFunService>(new BotFunService(botFunConfig))
					.AddTransient<IFetchPlayerStatsOperation, ApiFetchPlayerStatsOperation>()
					.AddTransient<IFindPlayerOperation, ApiFindPlayerOperation>()
					.AddTransient<ISearchUnitDatabaseOperation, ApiSearchUnitDatabaseOpeartion>()
					.AddTransient<IPlayerService, OperationPlayerService>()
					.AddTransient<ISearchMapOperation, ApiSearchMapOperation>()
					.AddTransient<IFetchLadderPoolOperation, ApiFetchLadderPoolOperation>();

				services.AddHttpClient<ApiClient>(client =>
				{
					client.BaseAddress = ApiUri;
				});

				services.AddHttpClient<UnitClient>(client =>
				{
					client.BaseAddress = new Uri(UnitDbUtils.UnitApi);
				});

				await using var serviceProvider = services.BuildServiceProvider();

				await ApplyDatabaseMigrations(serviceProvider.GetRequiredService<QAIDatabaseModel>());

				var user = Environment.GetEnvironmentVariable("IRC_USER");
				var pass = Environment.GetEnvironmentVariable("IRC_PASS");
				IrcConfiguration ircConfig = new()
				{
					Connection = Environment.GetEnvironmentVariable("IRC_CONN_DEST"),
					UserName = user,
					NickName = user,
					RealName = user,
					Password = pass
				};

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

				DiscordBotConfiguration discordConfig = new()
				{
					Prefix = Environment.GetEnvironmentVariable("BOT_PREFIX"),
					Shards = 1,
					Token = Environment.GetEnvironmentVariable("DISCORD_TOKEN")
				};

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

		private static async Task ApplyDatabaseMigrations(DbContext database)
		{
			if (!(await database.Database.GetPendingMigrationsAsync()).Any())
			{
				return;
			}

			await database.Database.MigrateAsync();
			await database.SaveChangesAsync();
		}
	}
}