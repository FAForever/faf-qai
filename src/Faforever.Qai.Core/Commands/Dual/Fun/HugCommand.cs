using System.Threading.Tasks;

using Faforever.Qai.Core.Commands.Arguments;
using Faforever.Qai.Core.Commands.Context;

using Qmmands;

namespace Faforever.Qai.Core.Commands.Dual.Fun
{
    public class HugCommand : DualCommandModule
    {
        [Command("hug")]
        [Description("Hug a user")]
        public async Task HugCommandAsync(IBotUserCapsule? user = null)
        {
            string username = "";

            if (user is not null)
            {
                username = user.Username;
            }
            else
            {
                if (Context is DiscordCommandContext dctx)
                    username = dctx.User.Mention;
                else if (Context is IRCCommandContext ictx)
                    username = ictx.Author.NickName;
            }

            await Context.SendActionAsync($" hugs {username}");
        }
    }
}
