using Faforever.Qai.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System.Threading.Tasks;

namespace Faforever.Qai.Core.Tests.Operations
{
    public class PlayerOperationTest : OperationTestBase
    {
        private IPlayerService playerService { get; set; }
        private IServiceScope scope { get; set; }

        [OneTimeSetUp]
        public void FetchReplaySetUp()
        {
            scope = Services.CreateScope();
            playerService = scope.ServiceProvider.GetRequiredService<IPlayerService>();
        }

        [OneTimeTearDown]
        public void FetchReplayTearDown()
        {
            scope?.Dispose();
        }

        [TestCase("Crotalus_Bureus", TestName = "Find user with name", Author = "Crotalus_Bureus")]
        public async Task FindPlayer(string user)
        {
            var res = await playerService.FindPlayer(user);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Usernames, Does.Contain(user));
        }

        [TestCase("Crotalus_Bureus", TestName = "Get player stats", Author = "Crotalus_Bureus")]
        public async Task GetPlayerStats(string user)
        {
            var res = await playerService.FetchPlayerStats(user);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Name, Is.EqualTo(user));
        }

        [TestCase("Crotalus_Bureus", TestName = "Get player rating history", Author = "Crotalus_Bureus")]
        [Ignore("This is a test for the chart generation, disabled to reduce calls to API")]
        public async Task GetPlayerRatingHistory(string user)
        {
            var res = await playerService.GetRatingHistory(user, Constants.FafLeaderboard.Global);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Length, Is.GreaterThan(0));
        }

        [TestCase("Crotalus_Bureus", TestName = "Generate rating chart", Author = "Crotalus_Bureus")]
        [Ignore("This is a test for the chart generation, disabled to reduce calls to API")]
        public async Task GenerateRatingChart(string user)
        {
            var res = await playerService.GenerateRatingChart(user, Constants.FafLeaderboard.Global);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Length, Is.GreaterThan(0));
        }
    }
}
