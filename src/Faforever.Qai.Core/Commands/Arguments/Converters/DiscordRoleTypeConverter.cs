using System.Threading.Tasks;

using DSharpPlus.Entities;

using Faforever.Qai.Core.Commands.Context;

using Qmmands;

namespace Faforever.Qai.Core.Commands.Arguments.Converters
{
    public class DiscordRoleTypeConverter : TypeParser<DiscordRole>
    {
        public override ValueTask<TypeParserResult<DiscordRole>> ParseAsync(Parameter parameter, string value, CommandContext context)
        {
            if (!(context is DiscordCommandContext ctx))
                return TypeParserResult<DiscordRole>.Failed("Context failed to parse to DiscordCommandContext");

            var valToParse = value;

            if (valToParse.StartsWith("<@&"))
                valToParse = valToParse.Replace("<@&", string.Empty);

            if (valToParse.EndsWith(">"))
                valToParse = valToParse.Replace(">", string.Empty);

            if (ulong.TryParse(valToParse, out ulong res))
            {
                var role = ctx.Guild.GetRole(res);
                if (role is null)
                    return TypeParserResult<DiscordRole>.Failed("Failed to get a role.");

                return TypeParserResult<DiscordRole>.Successful(role);
            }
            else return TypeParserResult<DiscordRole>.Failed("Failed to get a valid role ID.");
        }
    }
}
