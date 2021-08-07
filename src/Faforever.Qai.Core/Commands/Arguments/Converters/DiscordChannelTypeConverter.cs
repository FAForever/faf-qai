using System.Threading.Tasks;

using DSharpPlus.Entities;

using Faforever.Qai.Core.Commands.Context;

using Qmmands;

namespace Faforever.Qai.Core.Commands.Arguments.Converters
{
    public class DiscordChannelTypeConverter : TypeParser<DiscordChannel>
    {
        public override async ValueTask<TypeParserResult<DiscordChannel>> ParseAsync(Parameter parameter, string value, CommandContext context)
        {
            if (!(context is DiscordCommandContext ctx))
                return TypeParserResult<DiscordChannel>.Failed("Context failed to parse to DiscordCommandContext");

            var valToParse = value;

            if (valToParse.StartsWith("<#"))
                valToParse = valToParse.Replace("<#", string.Empty);

            if (valToParse.EndsWith(">"))
                valToParse = valToParse.Replace(">", string.Empty);

            if (ulong.TryParse(valToParse, out ulong res))
            {
                var chan = await ctx.Client.GetChannelAsync(res);
                if (chan is null)
                    return TypeParserResult<DiscordChannel>.Failed("Failed to get a channel.");

                return TypeParserResult<DiscordChannel>.Successful(chan);
            }
            else return TypeParserResult<DiscordChannel>.Failed("Failed to get a valid channel ID.");
        }
    }
}
