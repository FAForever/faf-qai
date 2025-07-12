using System.Linq;
using System.Threading.Tasks;
using Faforever.Qai.Core.Operations.Replays;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using NUnit.Framework.Legacy;

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

        [TestCase("9066822", TestName = "Get Replay By ID", Author = "Crotalus_Bureus")]
        public async Task VerifyReplayIDsMatch(long replayId)
        {
            var res = await Replay.FetchReplayAsync(replayId);
            Assert.That(res, Is.Not.Null);
            Assert.That(replayId, Is.EqualTo(res.Id), $"Expected {replayId} got {res.Id}");
        }

        [TestCase("Crotalus_Bureus", TestName = "Get Last Replay For User", Author = "Crotalus_Bureus")]
        public async Task GetLastReplayOfMember(string user)
        {
            var res = await Replay.FetchLastReplayAsync(user);

            Assert.That(res, Is.Not.Null);

            var players = (from data in res.PlayerStats
                           where data.Player.Login.ToLower() == user.ToLower()
                           select data).ToArray();

            Assert.That(players.Length > 0, Is.True, $"Expected one player from the match to be {user}");
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
