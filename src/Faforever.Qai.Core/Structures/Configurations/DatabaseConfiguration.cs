
using Newtonsoft.Json;

namespace Faforever.Qai.Core.Structures.Configurations
{
	public class DatabaseConfiguration
	{
		[JsonProperty("data_source")]
		public string DataSource { get; set; } = default!;
	}
}
