using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using Faforever.Qai.Core.Database;
using Faforever.Qai.Core.Services;
using Faforever.Qai.Core.Structures.Configurations;

namespace Faforever.Qai.Discord.Commands.Moderation.Relay
{
	public class RemoveRelayCommand : CommandModule
	{
		private readonly RelayService _relay;
		private readonly QAIDatabaseModel _database;

		public RemoveRelayCommand(RelayService relay, QAIDatabaseModel database)
		{
			this._relay = relay;
			this._database = database;
		}

		[Command("removerelay")]
		[Description("Removes a registered relay from your server..")]
		[Aliases("delrelay")]
		[RequireUserPermissions(Permissions.ManageChannels)]
		public async Task RemoveRelayCommandAsync(CommandContext ctx,
			[Description("IRC channel to remove relays for.")]
			string ircChannel)
		{
			var cfg = await _database.FindAsync<RelayConfiguration>(ctx.Guild.Id);
			if (cfg is null || !cfg.Webhooks.TryGetValue(ircChannel, out var hook))
			{
				await RespondBasicError("No relays found.");
				return;
			}

			await RemoveRelayCommandAsync(ctx, hook.Id);
		}

		[Command("removerelay")]
		[Priority(2)]
		public async Task RemoveRelayCommandAsync(CommandContext ctx,
			[Description("Discord Channel to remove relay for.")]
			DiscordChannel discordChannel)
		{
			var cfg = await _database.FindAsync<RelayConfiguration>(ctx.Guild.Id);
			if (cfg is null)
			{
				await RespondBasicError("No relays found.");
				return;
			}

			var discordHooks = await discordChannel.GetWebhooksAsync();

			foreach (var hook in discordHooks)
			{
				if (cfg.DiscordToIRCLinks.TryGetValue(hook.Id, out var data))
				{
					if (cfg.Webhooks.TryGetValue(data, out var hookData))
					{
						await RemoveRelayCommandAsync(ctx, hookData.Id);
						return;
					}
				}
			}

			await RespondBasicError("No relays found.");
		}

		private async Task RemoveRelayCommandAsync(CommandContext ctx, ulong webhookId)
		{
			var res = await _relay.RemoveRelayAsync(ctx.Guild.Id, webhookId);
			if (!(res is null))
			{
				var discordHook = await ctx.Client.GetWebhookAsync(res.Id);
				await discordHook.DeleteAsync();

				await RespondBasicSuccess("Relay removed succesfuly.");
			}
			else
			{
				await RespondBasicError("No relay found for that channel.");
			}
		}
	}
}
