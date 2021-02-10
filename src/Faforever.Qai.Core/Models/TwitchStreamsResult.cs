using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace Faforever.Qai.Core.Models
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	public class TwitchStreamsResult
	{
		[JsonProperty("data")]
		public IReadOnlyList<TwitchStream> Streams { get; internal set; }
		[JsonProperty("pagination")]
		public TwitchPagination Pagination { get; internal set; }
	}

	public struct TwitchPagination
	{
		[JsonProperty("cursor")]
		public string Cursor { get; set; }
	}

	public class TwitchStream
	{
		[JsonProperty("title")]
		public string Title { get; internal set; }
		[JsonProperty("user_login")]
		public string UserLogin { get; internal set; }
		[JsonIgnore]
		public string StreamLink
		{
			get
			{
				return $"https://twitch.tv/{UserLogin}";
			}
		}
	}

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
