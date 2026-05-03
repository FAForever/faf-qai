using System.Threading.Tasks;
using Faforever.Qai.Core.Clients;
using Faforever.Qai.Core.Models;

using Newtonsoft.Json;

namespace Faforever.Qai.Core.Operations.Units
{
    public class UnitDbSearchUnitDatabaseOpeartion(UnitClient units) : ISearchUnitDatabaseOperation
    {
        public async Task<UnitDatabaseSerachResult?> SearchUnitDatabase(string serach)
        {
            string? json = await units.Client
                .GetStringAsync($"?searchunit={serach}");

            var result = JsonConvert.DeserializeObject<UnitDatabaseSerachResult>(json);

            if (result is null) return null;

            // Chop out the < > text.
            result.Description = result.Description.Substring(result.Description.IndexOf(">") + 1);

            if (result.GeneralData.UnitName is not null)
                result.GeneralData.UnitName = result.GeneralData.UnitName.Substring(result.GeneralData.UnitName.IndexOf(">") + 1);

            return result;
        }
    }
}
