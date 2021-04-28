using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Faforever.Qai.Core.Operations.Replays;

using Microsoft.Extensions.DependencyInjection;

using NUnit.Framework;

namespace Faforever.Qai.Core.Tests.Operations
{
	public class FetchReplayTest : OperationTestBase
	{
		private IFetchReplayOperation Replay { get; set; }
		private IServiceScope Scope { get; set; }

		[OneTimeSetUp]
		public void FetchReplaySetUp()
		{
			Scope = Services.CreateScope();
			Replay = Scope.ServiceProvider.GetRequiredService<IFetchReplayOperation>();
		}

		[TestCase("13670943", TestName = "Get Replay By ID", Author = "Soyvolon")]
		public async Task VerifyReplayIDsMatch(string replayId)
		{
			var res = await Replay.FetchReplayAsync(replayId);
			Assert.NotNull(res);
			Assert.AreEqual(res.Id.ToString(), replayId, message: $"Expected {replayId} got {res.Id}");
		}

		[TestCase("Soyvolon", TestName = "Get Last Replay For User", Author = "Soyvolon")]
		public async Task GetLastReplayOfMember(string user)
		{
			var res = await Replay.FetchLastReplayAsync(user);

			Assert.NotNull(res);

			var players = (from data in res.PlayerData
						 where data.Name.ToLower() == user.ToLower()
						 select data).ToArray();

			Assert.True(players.Length > 0, message: $"Expected one player from the match to be {user}");
		}

		[OneTimeTearDown]
		public void FetchReplayTearDown()
		{
			Replay = null;
			Scope.Dispose();
			Scope = null;
		}
	}
}
