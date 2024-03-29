using System.Linq;
using System.Threading.Tasks;

using Faforever.Qai.Core.Commands.Context;

using Qmmands;

using static Faforever.Qai.Core.Commands.Arguments.Converters.ConverterUtils;

namespace Faforever.Qai.Core.Commands.Arguments.Converters
{
    public class BotUserCapsuleConverter : TypeParser<IBotUserCapsule>
    {
        public override async ValueTask<TypeParserResult<IBotUserCapsule>> ParseAsync(Parameter parameter, string value, CommandContext context)
        {
            if (context is IrcCommandContext ircCtx)
            {
                var cap = GetIrcUserCapsule(parameter, value, ircCtx);
                if (cap is not null)
                    return TypeParserResult<IBotUserCapsule>.Successful(cap);
            }
            else if (context is DiscordCommandContext disCtx)
            {
                var hasUserCapsuleArg = disCtx.Command.Parameters.Any(x => x.Type == typeof(IBotUserCapsule));
                if (hasUserCapsuleArg) { 
                    var id = ParseDiscordUserId(value);

                    if (id is null) return TypeParserResult<IBotUserCapsule>.Failed("Failed to get a valid discord ID.");

                    var cap = await GetDiscordUserOrMemberCapsule(id.Value, disCtx);
                    if (cap is not null)
                        return TypeParserResult<IBotUserCapsule>.Successful(cap);
                    }
            }

            return TypeParserResult<IBotUserCapsule>.Failed("Failed to get a valid user.");
        }
    }
}
