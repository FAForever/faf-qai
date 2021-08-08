using System.Linq;
using System.Threading.Tasks;

using DSharpPlus.Entities;

using Faforever.Qai.Core.Commands.Context;

using Qmmands;

namespace Faforever.Qai.Core.Commands.Arguments.Converters
{
    public static class ConverterUtils
    {
        public static IrcUserCapsule? GetIrcUserCapsule(Parameter p, string v, IrcCommandContext ctx)
        {
            var users = ctx.LocalUser.GetChannelUsers();
            
            var user = users.FirstOrDefault(x => x.User.NickName == v);
            if (user is not null)
                return new IrcUserCapsule(user.User);

            return null;
        }

        public static ulong? GetDiscordUserId(Parameter p, string v)
        {
            var valToParse = v;

            if (valToParse.StartsWith("<@!") || valToParse.StartsWith("<!@"))
                valToParse = valToParse[3..(valToParse.Length - 1)];

            if (valToParse.StartsWith("<@"))
                valToParse = valToParse[2..(valToParse.Length - 1)];

            if (valToParse.EndsWith(">"))
                valToParse = valToParse[..(valToParse.Length - 2)];

            if (ulong.TryParse(valToParse, out var id))
            {
                return id;
            }

            return null;
        }

        public static async Task<DiscordMember?> GetDiscordMember(ulong id, DiscordCommandContext ctx)
        {
            var member = await ctx.Guild.GetMemberAsync(id);

            return member;
        }

        public static async Task<DiscordUser?> GetDiscordUser(ulong id, DiscordCommandContext ctx)
        {
            var user = await ctx.Client.GetUserAsync(id);

            return user;
        }

        public static async Task<IBotUserCapsule?> GetDiscordUserOrMemberCapsule(ulong id, DiscordCommandContext ctx)
        {
            var member = await GetDiscordMember(id, ctx);

            if (member is not null)
                return new DiscordMemberCapsule(member);

            var user = await GetDiscordUser(id, ctx);

            if (user is not null)
                return new DiscordUserCapsule(user);

            return null;
        }
    }
}
