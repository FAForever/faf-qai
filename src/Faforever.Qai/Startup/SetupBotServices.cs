using DSharpPlus;
using Faforever.Qai.Core;
using Faforever.Qai.Core.Commands.Arguments.Converters;
using Faforever.Qai.Core.Commands.Context;
using Faforever.Qai.Core.Database;
using Faforever.Qai.Core.Operations.Clan;
using Faforever.Qai.Core.Operations.Clients;
using Faforever.Qai.Core.Operations.Content;
using Faforever.Qai.Core.Operations.FafApi;
using Faforever.Qai.Core.Operations.Maps;
using Faforever.Qai.Core.Operations.Player;
using Faforever.Qai.Core.Operations.Replays;
using Faforever.Qai.Core.Operations.Units;
using Faforever.Qai.Core.Services;
using Faforever.Qai.Core.Services.BotFun;
using Faforever.Qai.Core.Structures.Configurations;
using Faforever.Qai.Discord;
using Faforever.Qai.Discord.Core.Structures.Configurations;
using Faforever.Qai.Discord.Utils.Bot;
using Faforever.Qai.Irc;
using IrcDotNet;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Qmmands;
using System;
using System.IO;
using System.Linq;

namespace Faforever.Qai.Startup
{
    public static partial class StartupExtensions
    {
        public static void SetupBotServices(this IServiceCollection services, BotConfig botConfig)
        {
#if DEBUG
            // For use when the DB is Database/db-name.db
            if (!Directory.Exists("Database"))
                Directory.CreateDirectory("Database");
#endif

            // HTTP Client Mapping
            services.AddHttpClient<ApiHttpClient>(client =>
            {
                client.BaseAddress = new Uri(botConfig.Faf.Api);
            });

            services.AddHttpClient<UnitClient>(client =>
            {
                client.BaseAddress = new Uri(UnitDbUtils.UnitApi);
            });

            services.AddHttpClient<TwitchClient>();

            var (botFunConfig, urlConfig) = GetJsonConfig();
            var twitchConfig = GetTwitchConfig(botConfig);

            services.AddDbContext<QAIDatabaseModel>(options =>
            {
                options.UseSqlite($"Data Source={botConfig.DataSource}");
            }, ServiceLifetime.Singleton, ServiceLifetime.Singleton);

            services
                .AddSingleton<RelayService>()
                // Command Service Registration
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
                    options.AddTypeParser(new DiscordRoleTypeConverter());
                    options.AddTypeParser(new BotUserCapsuleConverter());
                    options.AddTypeParser(new DiscordMemberConverter());
                    options.AddTypeParser(new DiscordUserConverter());
                    return options;
                })
                .AddSingleton<QCommandsHandler>()
                .AddSingleton(typeof(TwitchClientConfig), twitchConfig)
                // Operation Service Registration
                .AddSingleton<IBotFunService>(new BotFunService(botFunConfig))
                .AddSingleton<IUrlService>(new UrlService(urlConfig))
                .AddSingleton<DiscordEventHandler>()
                .AddSingleton<AccountLinkService>()
                .AddTransient<IFetchPlayerStatsOperation, ApiFetchPlayerStatsOperation>()
                .AddTransient<IFindPlayerOperation, ApiFindPlayerOperation>()
                .AddTransient<ISearchUnitDatabaseOperation, UnitDbSearchUnitDatabaseOpeartion>()
                .AddTransient<IPlayerService, OperationPlayerService>()
                .AddTransient<GameService>()
                .AddTransient<ISearchMapOperation, ApiSearchMapOperation>()
                .AddTransient<IFetchLadderPoolOperation, ApiFetchLadderPoolOperation>()
                .AddTransient<IFetchReplayOperation, ApiFetchReplayOperation>()
                .AddTransient<IFetchClanOperation, ApiFetchClanOperation>()
                .AddTransient<IFetchTwitchStreamsOperation, FetchTwitchStreamsOperation>()
                .AddTransient<FafApiClient>();

            // Discord Information Setup
            var discordConfig = new DiscordBotConfiguration()
            {
                EnableSlashCommands = botConfig.Discord.SlashCommands,
                Prefix = botConfig.BotPrefix,
                Shards = 1,
                Token = botConfig.Discord.Token,
                FafStaff = botConfig.Discord.Roles.FafStaff,
            };

            var dcfg = new DiscordConfiguration
            {
                Token = discordConfig.Token,
                TokenType = TokenType.Bot,
                MinimumLogLevel = LogLevel.Debug,
                ShardCount = discordConfig.Shards, // Default to 1 for automatic sharding.
                Intents = DiscordIntents.Guilds | DiscordIntents.GuildMessages | DiscordIntents.DirectMessages | DiscordIntents.MessageContents,
            };

            services.AddSingleton(discordConfig)
                .AddSingleton<DiscordShardedClient>(x =>
                {
                    return new(dcfg);
                })
                .AddSingleton<DiscordRestClient>(x =>
                {
                    return new(dcfg);
                })
                .AddSingleton<DiscordBot>();

            // IRC Information Setup
            var user = botConfig.Irc.User;
            var channels = botConfig.Irc.Channels;
            if (string.IsNullOrEmpty(channels))
                channels = "aeolus,newbie";

            var ircConfig = new IrcConfiguration()
            {
                Connection = botConfig.Irc.Connection,
                Channels = channels.Split(',').Select(s => s.Trim()).ToArray(),
                UserName = user,
                NickName = user,
                RealName = user,
                Password = botConfig.Irc.Password
            };

            var ircConnInfo = new IrcUserRegistrationInfo
            {
                NickName = ircConfig.NickName,
                RealName = ircConfig.RealName,
                Password = ircConfig.Password,
                UserName = ircConfig.UserName
            };

            services.AddSingleton(ircConfig)
                .AddSingleton(ircConnInfo as IrcRegistrationInfo)
                .AddSingleton<QaIrc>();
        }

        private static (BotFunConfiguration botFunConfig, UrlConfiguration urlConfig) GetJsonConfig()
        {
            string json = File.ReadAllText(Path.Join("Config", "games_config.json"));
            var botFunConfig = JsonConvert.DeserializeObject<BotFunConfiguration>(json);

            json = File.ReadAllText(Path.Join("Config", "url_config.json"));
            var urlConfig = JsonConvert.DeserializeObject<UrlConfiguration>(json);

            return (botFunConfig, urlConfig);
        }

        private static TwitchClientConfig GetTwitchConfig(BotConfig config)
        {
            return new()
            {
                ClientId = config.Twitch?.ClientId,
                ClientSecret = config.Twitch?.ClientSecret
            };
        }
    }
}
