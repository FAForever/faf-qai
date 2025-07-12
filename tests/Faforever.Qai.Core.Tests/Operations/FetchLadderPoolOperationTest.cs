using Faforever.Qai.Core.Operations.Maps;
using Faforever.Qai.Core.Operations.PatchNotes;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using NUnit.Framework.Legacy;
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

        [OneTimeTearDown]
        public void FetchReplayTearDown()
        {
            scope?.Dispose();
        }

        [TestCase(TestName = "Fetch patch notes")]
        public async Task FetchPatchNotes()
        {
            var res = await operation.FetchLadderPoolAsync();

            Assert.That(res, Is.Not.Null);
            Assert.That(res, Is.Not.Empty);
        }
    }
}
