using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Faforever.Qai.Core.Operations.Clan;
using Faforever.Qai.Core.Operations.Clients;
using Faforever.Qai.Core.Operations.Content;
using Faforever.Qai.Core.Operations.Maps;
using Faforever.Qai.Core.Operations.Player;
using Faforever.Qai.Core.Operations.Replays;
using Faforever.Qai.Core.Operations.Units;
using Faforever.Qai.Core.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using NUnit.Framework;

namespace Faforever.Qai.Core.Tests.Operations
{
    public class OperationTestBase
    {
		protected IServiceProvider Services { get; private set; }

		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			ServiceCollection collection = new();
			collection.AddSingleton<IConfiguration>((x) =>
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

			var config = collection.BuildServiceProvider().GetRequiredService<IConfiguration>();

			collection
				.AddTransient<IFetchPlayerStatsOperation, ApiFetchPlayerStatsOperation>()
				.AddTransient<IFindPlayerOperation, ApiFindPlayerOperation>()
				.AddTransient<ISearchUnitDatabaseOperation, UnitDbSearchUnitDatabaseOpeartion>()
				.AddTransient<IPlayerService, OperationPlayerService>()
				.AddTransient<ISearchMapOperation, ApiSearchMapOperation>()
				.AddTransient<IFetchLadderPoolOperation, ApiFetchLadderPoolOperation>()
				.AddTransient<IFetchReplayOperation, ApiFetchReplayOperation>()
				.AddTransient<IFetchClanOperation, ApiFetchClanOperation>()
				.AddTransient<IFetchTwitchStreamsOperation, FetchTwitchStreamsOperation>();

			// HTTP Client Mapping
			collection.AddHttpClient<ApiHttpClient>(client =>
			{
				client.BaseAddress = new($"{config["Config:Faf:Api"]}");
			});

			collection.AddHttpClient<UnitClient>(client =>
			{
				client.BaseAddress = new Uri(UnitDbUtils.UnitApi);
			});

			collection.AddHttpClient<TwitchClient>();

			Services = collection.BuildServiceProvider();
		}

		[OneTimeTearDown]
		public void OneTimeTearDown()
		{

		}
    }
}
