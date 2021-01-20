using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Faforever.Qai.Core.Models;

namespace Faforever.Qai.Core.Operations.Clan
{
	public interface IFetchClanOperation
	{
		public Task<FetchClanResult?> FetchClanAsync(string clan);
	}
}
