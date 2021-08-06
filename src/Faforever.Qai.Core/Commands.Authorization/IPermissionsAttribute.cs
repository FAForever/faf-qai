namespace Faforever.Qai.Core.Commands.Authorization
{
	public interface IPermissionsAttribute
	{
		public DSharpPlus.Permissions? DiscordPermissions { get; }
		public IrcPermissions? IRCPermissions { get; }
	}
}
