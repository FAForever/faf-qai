using System.Threading.Tasks;
using Faforever.Qai.Core.Commands.Authorization;
using Qmmands;

namespace Faforever.Qai.Core.Commands.Irc
{
    public class IrcPermissionTesting : IrcCommandModule
    {
        [Command("ircbotop")]
        [Description("Test if bot has Aeolus moderator permissions")]
        [RequireBotPermissions(IrcPermissions.AeolusModerator)]
        public async Task BotOpCheck()
        {
            await RespondBasicSuccess("Bot have aeolus moderator permission!");
        }

        [Command("ircbotmod")]
        [Description("Test if bot has channel moderator permissions")]
        [RequireBotPermissions(IrcPermissions.ChannelModerator)]
        public async Task BotModCheck()
        {
            await RespondBasicSuccess("Bot have channel moderator permission!");
        }

        [Command("ircmod")]
        [Description("Test if user has channel moderator permissions")]
        [RequireUserPermissions(IrcPermissions.ChannelModerator)]
        public async Task UserModCheck()
        {
            await RespondBasicSuccess("User have channel moderator permission!");
        }

        [Command("ircop")]
        [Description("Test if user has Aeolus moderator permissions")]
        [RequireUserPermissions(IrcPermissions.AeolusModerator)]
        public async Task UserOpCheck()
        {
            await RespondBasicSuccess("User have aeolus moderator permission!");
        }

        [Command("ircbothperms")]
        [Description("Test if both user and bot have channel moderator permissions")]
        [RequirePermissions(IrcPermissions.ChannelModerator)]
        public async Task BothOpCheck()
        {
            await RespondBasicSuccess("User and bot have channel operator permission!");
        }
    }
}
