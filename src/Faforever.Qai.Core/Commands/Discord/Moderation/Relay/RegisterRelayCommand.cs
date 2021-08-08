using System.Linq;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.Entities;

using Faforever.Qai.Core.Commands.Authorization;
using Faforever.Qai.Core.Database;
using Faforever.Qai.Core.Database.Entities;
using Faforever.Qai.Core.Services;

using Qmmands;

namespace Faforever.Qai.Core.Commands.Moderation.Relay
{
    public class RegisterRelayCommand : DiscordCommandModule
    {
        private readonly RelayService _relay;
        private readonly QAIDatabaseModel _database;

        public RegisterRelayCommand(RelayService relay, QAIDatabaseModel database)
        {
            this._relay = relay;
            this._database = database;
        }

        [Command("registerrelay", "rrelay")]
        [Description("Registers a link between an IRC channel and a Discord channel.")]
        [RequireUserPermissions(Permissions.ManageChannels)]
        [RequireBotPermissions(Permissions.ManageWebhooks)]
        public async Task RegisterRelayCommandAsync(
            [Description("Discord channel to link to.")]
            DiscordChannel discordChannel,

            [Description("IRC channel to link to.")]
            string ircChannel)
        {
            ircChannel = GetIrcChannelName(ircChannel);

            if (Context.Channel.GuildId == null)
            {
                await RespondBasicError("Can only use relay command in a guild channel");
                return;
            }

            var cfg = _database.Find<RelayConfiguration>(Context.Channel.GuildId);
            if (cfg is null)
            {
                cfg = new RelayConfiguration()
                {
                    DiscordServer = Context.Channel.GuildId.Value
                };

                await _database.AddAsync(cfg);
                await _database.SaveChangesAsync();
            }

            // TODO: Some check to see if the IRC channel is available.
            if (cfg.DiscordToIRCLinks.ContainsKey(discordChannel.Id))
            {
                await RespondBasicError("A relay already exists for this Discord channel.");
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

                    var discordHook = await Context.Client.GetWebhookAsync(hook.Id);
                    await discordHook.ModifyAsync(channelId: discordChannel.Id);

                    await _database.SaveChangesAsync();

                    await RespondBasicSuccess($"Moved the relay for IRC channel `{ircChannel}` to {discordChannel.Name}");
                }

                return;
            }

            var newHook = await discordChannel.CreateWebhookAsync($"Dostya-{ircChannel}", reason: "Dostya Relay Creation");

            if (await _relay.AddRelayAsync(Context.Channel.GuildId.Value, newHook, ircChannel))
            {
                await RespondBasicSuccess($"Relay bridge added ({discordChannel.Name} <-> {ircChannel})");
            }
            else
            {
                await RespondBasicError("Failed to add new relay.");
            }
        }
    }
}
