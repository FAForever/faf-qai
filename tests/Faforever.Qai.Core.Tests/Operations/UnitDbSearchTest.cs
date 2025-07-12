using System.Threading.Tasks;

using Faforever.Qai.Core.Operations.Units;

using Microsoft.Extensions.DependencyInjection;

using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Faforever.Qai.Core.Tests.Operations
{
    public class UnitDbSearchTest : OperationTestBase
    {
        private ISearchUnitDatabaseOperation Search { get; set; }
        private IServiceScope Scope { get; set; }

        [OneTimeSetUp]
        public void UnitSearchSetUp()
        {
            Scope = Services.CreateScope();
            Search = Scope.ServiceProvider.GetRequiredService<ISearchUnitDatabaseOperation>();
        }

        [TestCase("fatboy", "UEL0401", TestName = "Test Get Fatboy", Author = "Soyvolon")]
        [TestCase("absolver", "DAL0310", TestName = "Test Get Absolver", Author = "Soyvolon")]
        public async Task SearchForUnitByStringTest(string unit, string expectedUnitId)
        {
            var res = await Search.SearchUnitDatabase(unit);
            Assert.That(expectedUnitId, Is.EqualTo(res.Id), $"Got {res.Id} expected {expectedUnitId}");
        }

        [TestCase("XSL0111", TestName = "Test Get Ythisah", Author = "Soyvolon")]
        [TestCase("URL0208", TestName = "Test Get Cybran T2 Engi", Author = "Soyvolon")]
        public async Task SearchForUnitByUnitID(string unitId)
        {
            var res = await Search.SearchUnitDatabase(unitId);
            Assert.That(unitId, Is.EqualTo(res.Id), $"Got {res.Id} expected {unitId}");
        }

        [TestCase("abcdefg", TestName = "Test Serach Invalid Unit", Author = "Soyvolon")]
        public async Task SearchForInvalidUnit(string unitId)
        {
            var res = await Search.SearchUnitDatabase(unitId);
            Assert.That(res, Is.Null, $"Expected null got {res?.Id}");
        }

        [OneTimeTearDown]
        public void UnitSearchTearDown()
        {
            Search = null;
            Scope.Dispose();
            Scope = null;
        }
    }
}
