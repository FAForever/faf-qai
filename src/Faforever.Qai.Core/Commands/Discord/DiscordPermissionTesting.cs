using System.Threading.Tasks;

using Faforever.Qai.Core.Commands.Authorization;

using Qmmands;

namespace Faforever.Qai.Core.Commands.Discord
{
    public class DiscordPermissionTesting : DiscordCommandModule
    {
        [Command("testbotperms")]
        [Description("Test if bot has webhook management permissions")]
        [RequireBotPermissions(DSharpPlus.Permissions.ManageWebhooks)]
        public async Task BotPermsCheck()
        {
            await RespondBasicSuccess("Bot have ManageWebhooks permission!");
        }

        [Command("testuserperms")]
        [Description("Test if user has administrator permissions")]
        [RequireUserPermissions(DSharpPlus.Permissions.Administrator)]
        public async Task UserPremsCheck()
        {
            await RespondBasicSuccess("User have Administrator permission!");
        }

        [Command("testbothperms")]
        [Description("Test if both user and bot have channel management permissions")]
        [RequirePermissions(DSharpPlus.Permissions.ManageChannels)]
        public async Task BothPermsCheck()
        {
            await RespondBasicSuccess("User and bot have ManageChannels permission!");
        }
    }
}
