using Faforever.Qai.Core.Operations.Maps;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System.Threading.Tasks;

namespace Faforever.Qai.Core.Tests.Operations
{
    public class SearchMapOperationTest : OperationTestBase
    {
        private ISearchMapOperation operation { get; set; }
        private IServiceScope scope { get; set; }

        [OneTimeSetUp]
        public void FetchReplaySetUp()
        {
            scope = Services.CreateScope();
            operation = scope.ServiceProvider.GetRequiredService<ISearchMapOperation>();
        }

        [OneTimeTearDown]
        public void FetchReplayTearDown()
        {
            scope?.Dispose();
        }

        [TestCase("DualGap_fix_adaptive", TestName = "Fetch map by name")]
        public async Task GetMapByName(string mapName)
        {
            var res = await operation.GetMapAsync(mapName);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.DisplayName, Is.Not.Empty);
        }
    }
}
