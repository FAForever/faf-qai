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

        [OneTimeTearDown]
        public void FetchReplayTearDown()
        {
            scope?.Dispose();
        }

        [TestCase("DAD", TestName = "Fetch clan")]
        public async Task FetchPatchNotes(string clanName)
        {
            var res = await operation.FetchClanAsync(clanName);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Clan.Tag, Is.EqualTo("DAD"));
        }
    }
}
