using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Faforever.Qai.Core.Commands.Authorization;
using Qmmands;

namespace Faforever.Qai.Core.Commands.Irc
{
    public class IrcPermissionTesting : IrcCommandModule
    {
        [Command("ircbotop")]
        [RequireBotPermissions(IrcPermissions.AeolusModerator)]
        public async Task BotOpCheck()
        {
            await RespondBasicSuccess("Bot have aeolus moderator permission!");
        }

        [Command("ircbotmod")]
        [RequireBotPermissions(IrcPermissions.ChannelModerator)]
        public async Task BotModCheck()
        {
            await RespondBasicSuccess("Bot have channel moderator permission!");
        }

        [Command("ircmod")]
        [RequireUserPermissions(IrcPermissions.ChannelModerator)]
        public async Task UserModCheck()
        {
            await RespondBasicSuccess("User have channel moderator permission!");
        }

        [Command("ircop")]
        [RequireUserPermissions(IrcPermissions.AeolusModerator)]
        public async Task UserOpCheck()
        {
            await RespondBasicSuccess("User have aeolus moderator permission!");
        }

        [Command("ircbothperms")]
        [RequirePermissions(IrcPermissions.ChannelModerator)]
        public async Task BothOpCheck()
        {
            await RespondBasicSuccess("User and bot have channel operator permission!");
        }
    }
}
