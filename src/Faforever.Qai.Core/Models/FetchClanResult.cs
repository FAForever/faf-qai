using System.Collections;
using System.Collections.Generic;

namespace Faforever.Qai.Core.Models {
	public record FetchClanResult {
		public string Tag { get; init; }
		public string Name { get; init; }
		public string Url { get; init; }
		public string Description { get; init; }
		public IEnumerable<string> PlayerNames { get; init; }
	}
}