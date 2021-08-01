using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus.Entities;

using Faforever.Qai.Core.Commands.Context;

using Qmmands;

using static Faforever.Qai.Core.Commands.Arguments.Converters.ConverterUtils;

namespace Faforever.Qai.Core.Commands.Arguments.Converters
{
	public class DiscordUserConverter : TypeParser<DiscordUser>
	{
		public override async ValueTask<TypeParserResult<DiscordUser>> ParseAsync(Parameter parameter, string value, CommandContext context)
		{
			if (context is DiscordCommandContext ctx)
			{
				var id = GetDiscordUserId(parameter, value);

				if(id is null) return TypeParserResult<DiscordUser>.Failed("Failed to parse a valid ID.");

				var user = await GetDiscordUser(id.Value, ctx);

				if(user is null) return TypeParserResult<DiscordUser>.Failed($"Failed to get a valid DiscordUser from {id.Value}.");

				return TypeParserResult<DiscordUser>.Successful(user);
			}

			return TypeParserResult<DiscordUser>.Failed("Can't get a Discord user from a non Discord client.");
		}
	}
}
