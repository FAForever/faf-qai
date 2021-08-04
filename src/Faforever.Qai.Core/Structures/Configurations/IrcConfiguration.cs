using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace Faforever.Qai.Core.Structures.Configurations
{
	public class IrcConfiguration
	{
		[JsonProperty("nickname")]
		public string NickName { get; set; } = default!;
		[JsonProperty("realname")]
		public string RealName { get; set; } = default!;
		[JsonProperty("password")]
		public string Password { get; set; } = default!;
		[JsonProperty("username")]
		public string UserName { get; set; } = default!;
		[JsonProperty("con_uri")]
		public string Connection { get; set; } = default!;
		[JsonProperty("channels")]
		public string[] Channels { get; set; } = default!;
	}
}
