using Faforever.Qai.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
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

        [TestCase("Crotalus_Bureus", TestName = "Find user with name", Author = "Crotalus_Bureus")]
        public async Task FindPlayer(string user)
        {
            var res = await playerService.FindPlayer(user);

            Assert.NotNull(res);
            Assert.Contains(user, res.Usernames);
        }

        [TestCase("Crotalus_Bureus", TestName = "Get player stats", Author = "Crotalus_Bureus")]
        public async Task GetPlayerStats(string user)
        {
            var res = await playerService.FetchPlayerStats(user);

            Assert.NotNull(res);
            Assert.AreEqual(user, res.Name);
        }
    }
}
