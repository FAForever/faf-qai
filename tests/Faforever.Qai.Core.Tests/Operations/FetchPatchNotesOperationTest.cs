using Faforever.Qai.Core.Operations.PatchNotes;
using Faforever.Qai.Core.Operations.Replays;
using Faforever.Qai.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Faforever.Qai.Core.Tests.Operations
{
    public class FetchPatchNotesOperationTest : OperationTestBase
    {
        private IFetchPatchNotesLinkOperation operation { get; set; }
        private IServiceScope scope { get; set; }

        [OneTimeSetUp]
        public void FetchReplaySetUp()
        {
            scope = Services.CreateScope();
            operation = scope.ServiceProvider.GetRequiredService<IFetchPatchNotesLinkOperation>();
        }

        [OneTimeTearDown]
        public void FetchReplayTearDown()
        {
            scope?.Dispose();
        }

        [TestCase(TestName = "Fetch patch notes")]
        public async Task FetchPatchNotes()
        {
            var res = await operation.GetPatchNotesLinkAsync();

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Url, Is.Not.Null);
        }
    }
}
