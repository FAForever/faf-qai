using Faforever.Qai.Core.Operations.Maps;
using Faforever.Qai.Core.Operations.PatchNotes;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Faforever.Qai.Core.Tests.Operations
{
    public class FetchLadderPoolOperationTest : OperationTestBase
    {
        private IFetchLadderPoolOperation operation { get; set; }
        private IServiceScope scope { get; set; }

        [OneTimeSetUp]
        public void FetchReplaySetUp()
        {
            scope = Services.CreateScope();
            operation = scope.ServiceProvider.GetRequiredService<IFetchLadderPoolOperation>();
        }

        [TestCase(TestName = "Fetch patch notes")]
        public async Task FetchPatchNotes()
        {
            var res = await operation.FetchLadderPoolAsync();

            Assert.NotNull(res);
            Assert.IsNotEmpty(res);
        }
    }
}
