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
    public class RemoveRelayCommand : DiscordCommandModule
    {
        private readonly RelayService _relay;
        private readonly QAIDatabaseModel _database;

        public RemoveRelayCommand(RelayService relay, QAIDatabaseModel database)
        {
            this._relay = relay;
            this._database = database;
        }

        [Command("removerelay", "delrelay")]
        [Description("Removes a registered relay from your server.")]
        [RequirePermissions(Permissions.ManageChannels)]
        public async Task RemoveRelayCommandAsync(
            [Description("IRC channel to remove relays for.")]
            string ircChannel)
        {
            var cfg = await _database.FindAsync<RelayConfiguration>(Context.Channel.GuildId);
            if (cfg is null || !cfg.Webhooks.TryGetValue(ircChannel, out var hook))
            {
                await RespondBasicError("No relays found.");
                return;
            }

            await RemoveRelayCommandAsync(hook.Id);
        }

        [Command("removerelay", "delrelay")]
        [Priority(2)]
        public async Task RemoveRelayCommandAsync(
            [Description("Discord Channel to remove relay for.")]
            DiscordChannel discordChannel)
        {
            var cfg = await _database.FindAsync<RelayConfiguration>(Context.Channel.GuildId);
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
                        await RemoveRelayCommandAsync(hookData.Id);
                        return;
                    }
                }
            }

            await RespondBasicError("No relays found.");
        }

        private async Task RemoveRelayCommandAsync(ulong webhookId)
        {
            if (Context.Channel.GuildId == null)
                return;

            var res = await _relay.RemoveRelayAsync(Context.Channel.GuildId.Value, webhookId);

            if (!res)
            {
                await RespondBasicSuccess("Relay removed succesfully.");
            }
            else
            {
                await RespondBasicError("No relay found for that channel.");
            }
        }
    }
}
