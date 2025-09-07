using System;
using System.IO;
using Faforever.Qai.Core.Clients;
using Faforever.Qai.Core.Clients.QuickChart;
using Faforever.Qai.Core.Http;
using Faforever.Qai.Core.Operations.Clan;
using Faforever.Qai.Core.Operations.Content;
using Faforever.Qai.Core.Operations.FafApi;
using Faforever.Qai.Core.Operations.Maps;
using Faforever.Qai.Core.Operations.PatchNotes;
using Faforever.Qai.Core.Operations.Player;
using Faforever.Qai.Core.Operations.Replays;
using Faforever.Qai.Core.Operations.Units;
using Faforever.Qai.Core.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using NUnit.Framework;

namespace Faforever.Qai.Core.Tests.Operations
{
    public abstract class OperationTestBase
    {
#pragma warning disable NUnit1032 // An IDisposable field/property should be Disposed in a TearDown method
        protected IServiceProvider Services { get; private set; }
#pragma warning restore NUnit1032 // An IDisposable field/property should be Disposed in a TearDown method

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            ServiceCollection services = new();

            services.AddSingleton<IConfiguration>((x) =>
            {
                return new ConfigurationBuilder()
                    .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
#if DEBUG
                        .AddJsonFile("appsettings.Development.json", false)
#else
                        .AddJsonFile("appsettings.json", false)
#endif
                        .Build();
            });

            var config = services.BuildServiceProvider().GetRequiredService<IConfiguration>();

            services.AddMemoryCache();

            services
                .AddTransient<IFetchPlayerStatsOperation, ApiFetchPlayerStatsOperation>()
                .AddTransient<IFetchPatchNotesLinkOperation, FetchHtmlPatchNotesOperation>()
                .AddTransient<IFindPlayerOperation, ApiFindPlayerOperation>()
                .AddTransient<ISearchUnitDatabaseOperation, UnitDbSearchUnitDatabaseOpeartion>()
                .AddTransient<IPlayerService, OperationPlayerService>()
                .AddTransient<PlayerStatsCalculator>()
                .AddTransient<GameService>()
                .AddTransient<ISearchMapOperation, ApiSearchMapOperation>()
                .AddTransient<IFetchLadderPoolOperation, ApiFetchLadderPoolOperation>()
                .AddTransient<IFetchReplayOperation, ApiFetchReplayOperation>()
                .AddTransient<IFetchClanOperation, ApiFetchClanOperation>()
                .AddTransient<IFetchTwitchStreamsOperation, FetchTwitchStreamsOperation>()
                .AddTransient<FafApiClient>()
                .AddTransient<QuickChartClient>();

            // HTTP Client Mapping
            services.AddHttpClient<ApiHttpClient>(client =>
            {
                client.BaseAddress = new($"{config["Config:Faf:Api"]}");
            }).AddHttpMessageHandler(() => new OAuthHandler(new() {
                ClientId = config["Config:FafApi:ClientId"],
                ClientSecret = config["Config:FafApi:ClientSecret"]
            }));

            services.AddHttpClient<UnitClient>(client =>
            {
                client.BaseAddress = new Uri(UnitDbUtils.UnitApi);
            });

            services.AddHttpClient<TwitchClient>();

            Services = services.BuildServiceProvider();
        }
    }
}
