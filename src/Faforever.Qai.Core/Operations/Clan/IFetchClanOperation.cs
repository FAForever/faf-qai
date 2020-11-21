using System.Threading.Tasks;
using Faforever.Qai.Core.Models;

namespace Faforever.Qai.Core.Operations.Clan {
	public interface IFetchClanOperation {
		Task<FetchClanResult> FetchClan(string tag);
	}
}