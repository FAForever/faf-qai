using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Faforever.Qai.Core.Models;

namespace Faforever.Qai.Core.Operations.Replays
{
	public interface IFetchReplayOperation
	{
		public Task<ReplayResult?> FetchReplayAsync(string replayId);
		public Task<ReplayResult?> FetchLastReplayAsync(string username);
	}
}
