using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Faforever.Qai.Core.Models;

namespace Faforever.Qai.Core.Operations.Maps
{
	public interface ISearchMapOperation
	{
		public Task<MapResult?> GetMapAsync(string map);
		public Task<MapResult?> GetMapAsync(int mapId);
	}
}
