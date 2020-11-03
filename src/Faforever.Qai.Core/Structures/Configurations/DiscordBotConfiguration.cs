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
		public string Token { get; set; }
		/// <summary>
		/// The default prefix reconized by the bot.
		/// </summary>
		public string Prefix { get; set; }
		/// <summary>
		/// The default amount of shards to load. Set to 1 for automatic sharding.
		/// </summary>
		public int Shards { get; set; }
	}
}