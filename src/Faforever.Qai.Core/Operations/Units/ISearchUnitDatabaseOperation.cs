using System.Threading.Tasks;

using Faforever.Qai.Core.Models;

namespace Faforever.Qai.Core.Operations.Units
{
	public interface ISearchUnitDatabaseOperation
	{
		public Task<UnitDatabaseSerachResult?> SearchUnitDatabase(string serach);
	}
}
