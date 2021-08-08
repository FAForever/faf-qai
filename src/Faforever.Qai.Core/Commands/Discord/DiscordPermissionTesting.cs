using System.Threading.Tasks;

using Faforever.Qai.Core.Commands.Authorization;

using Qmmands;

namespace Faforever.Qai.Core.Commands.Discord
{
    public class DiscordPermissionTesting : DiscordCommandModule
    {
        [Command("testbotperms")]
        [RequireBotPermissions(DSharpPlus.Permissions.ManageWebhooks)]
        public async Task BotPermsCheck()
        {
            await RespondBasicSuccess("Bot have ManageWebhooks permission!");
        }

        [Command("testuserperms")]
        [RequireUserPermissions(DSharpPlus.Permissions.Administrator)]
        public async Task UserPremsCheck()
        {
            await RespondBasicSuccess("User have Administrator permission!");
        }

        [Command("testbothperms")]
        [RequirePermissions(DSharpPlus.Permissions.ManageChannels)]
        public async Task BothPermsCheck()
        {
            await RespondBasicSuccess("User and bot have ManageChannels permission!");
        }
    }
}
