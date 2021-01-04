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
		public string NickName { get; set; }
		[JsonProperty("realname")]
		public string RealName { get; set; }
		[JsonProperty("password")]
		public string Password { get; set; }
		[JsonProperty("username")]
		public string UserName { get; set; }
		[JsonProperty("con_uri")]
		public string Connection { get; set; }
	}
}
