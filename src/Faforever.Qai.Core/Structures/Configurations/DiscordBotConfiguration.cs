using Newtonsoft.Json;

namespace Faforever.Qai.Discord.Core.Structures.Configurations
{
	/// <summary>
	/// Holds configuration infromation for a Discord Bot
	/// </summary>
	public struct DiscordBotConfiguration
	{
		/// <summary>
		/// The Bot Token used to connect to Discord.
		/// </summary>
		[JsonProperty("token")]
		public string Token { get; set; }
		/// <summary>
		/// The default prefix reconized by the bot.
		/// </summary>
		[JsonProperty("prefix")]
		public string Prefix { get; set; }
		/// <summary>
		/// The default amount of shards to load. Set to 1 for automatic sharding.
		/// </summary>
		[JsonProperty("shards")]
		public int Shards { get; set; }
	}
}