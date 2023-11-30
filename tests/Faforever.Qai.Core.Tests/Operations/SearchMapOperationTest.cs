using Faforever.Qai.Core.Operations.Maps;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
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

        [TestCase("DualGap_fix_adaptive", TestName = "Fetch map by name")]
        public async Task GetMapByName(string mapName)
        {
            var res = await operation.GetMapAsync(mapName);

            Assert.NotNull(res);
            Assert.IsNotEmpty(res.DisplayName);
        }
    }
}
