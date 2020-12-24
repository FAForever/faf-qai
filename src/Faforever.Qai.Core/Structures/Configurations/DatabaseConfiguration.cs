using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace Faforever.Qai.Core.Structures.Configurations
{
	public class DatabaseConfiguration
	{
		[JsonProperty("data_source")]
		public string DataSource { get; set; }
	}
}
