using Faforever.Qai.Core.Operations.Clan;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Faforever.Qai.Core.Tests.Operations
{
    public class FetchClanOperationTest : OperationTestBase
    {
        private IFetchClanOperation operation;
        private IServiceScope scope;

        [OneTimeSetUp]
        public void FetchReplaySetUp()
        {
            scope = Services.CreateScope();
            operation = scope.ServiceProvider.GetRequiredService<IFetchClanOperation>();
        }

        [TestCase("DAD", TestName = "Fetch clan")]
        public async Task FetchPatchNotes(string clanName)
        {
            var res = await operation.FetchClanAsync(clanName);

            Assert.NotNull(res);
            Assert.AreEqual(res.Clan.Tag, "DAD");
        }
    }
}
