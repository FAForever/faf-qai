using System.Linq;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using Faforever.Qai.Core.Database;
using Faforever.Qai.Core.Services;
using Faforever.Qai.Core.Structures.Configurations;
using Faforever.Qai.Core.Structures.Webhooks;

using Microsoft.EntityFrameworkCore.Internal;

namespace Faforever.Qai.Discord.Commands.Moderation.Relay
{
	public class RegisterRelayCommand : CommandModule
	{
		private readonly RelayService _relay;
		private readonly QAIDatabaseModel _database;

		public RegisterRelayCommand(RelayService relay, QAIDatabaseModel database)
		{
			this._relay = relay;
			this._database = database;
		}

		[Command("registerrelay")]
		[Description("Registers a link between an IRC channel and a Discord channel.")]
		[Aliases("rrelay")]
		[RequireUserPermissions(Permissions.ManageChannels)]
		public async Task ExampleCommandAsync(CommandContext ctx,
			[Description("Discord channel to link to.")]
			DiscordChannel discordChannel,

			[Description("IRC channel to link to.")]
			string ircChannel)
		{
			var cfg = _database.Find<RelayConfiguration>(ctx.Guild.Id);
			if (cfg is null)
			{
				cfg = new RelayConfiguration()
				{
					DiscordServer = ctx.Guild.Id
				};

				await _database.AddAsync(cfg);
				await _database.SaveChangesAsync();
			}

			// TODO: Some check to see if the IRC channel is avalible.
			if (cfg.DiscordToIRCLinks.ContainsKey(discordChannel.Id))
			{
				await RespondBasicError("A relay already exsists for this Discord channel.");
			}

			var activeHooks = await discordChannel.GetWebhooksAsync();

			if (cfg.Webhooks.TryGetValue(ircChannel, out var hook))
			{
				if (activeHooks.Any(x => x.Id == hook.Id))
				{
					await RespondBasicError("A relay for this IRC channel is already in that Discord channel!");
				}
				else
				{
					_database.Update(cfg);

					var discordHook = await ctx.Client.GetWebhookAsync(hook.Id);
					await discordHook.ModifyAsync(channelId: discordChannel.Id);

					await _database.SaveChangesAsync();

					await RespondBasicSuccess($"Moved the relay for IRC channel `{ircChannel}` to {discordChannel.Mention}");
				}

				return;
			}

			var newHook = await discordChannel.CreateWebhookAsync($"Dostya-{ircChannel}", reason: "Dostya Relay Creation");

			var newHookData = new DiscordWebhookData()
			{
				Id = newHook.Id,
				Token = newHook.Token
			};

			if (await _relay.AddRelayAsync(ctx.Guild.Id, newHookData, ircChannel))
			{
				await RespondBasicSuccess("Relay added!");
			}
			else
			{
				await RespondBasicError("Failed to add new relay.");
			}
		}
	}
}
