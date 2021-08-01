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
	public class DiscordMemberConverter : TypeParser<DiscordMember>
	{
		public override async ValueTask<TypeParserResult<DiscordMember>> ParseAsync(Parameter parameter, string value, CommandContext context)
		{
			if (context is DiscordCommandContext ctx)
			{
				var id = GetDiscordUserId(parameter, value);

				if (id is null) return TypeParserResult<DiscordMember>.Failed("Failed to parse a valid ID.");

				var user = await GetDiscordMember(id.Value, ctx);

				if (user is null) return TypeParserResult<DiscordMember>.Failed($"Failed to get a valid DiscordMember from {id.Value}.");

				return TypeParserResult<DiscordMember>.Successful(user);
			}

			return TypeParserResult<DiscordMember>.Failed("Can't get a Discord member from a non Discord client.");
		}
	}
}
