using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus.Entities;

using Faforever.Qai.Core.Commands.Context;

using Qmmands;

namespace Faforever.Qai.Core.Commands.Arguments
{
	public class DiscordChannelTypeConverter : TypeParser<DiscordChannel>
	{
		public override async ValueTask<TypeParserResult<DiscordChannel>> ParseAsync(Parameter parameter, string value, CommandContext context)
		{
			if (!(context is DiscordCommandContext ctx))
				return TypeParserResult<DiscordChannel>.Unsuccessful("Context failed to parse to DiscordCommandContext");

			var valToParse = value;

			if (value.StartsWith("<#"))
				valToParse = valToParse.Replace("<#", string.Empty);

			if (value.EndsWith(">"))
				valToParse = valToParse.Replace(">", string.Empty);

			if (ulong.TryParse(valToParse, out ulong res))
			{
				var chan = await ctx.Client.GetChannelAsync(res);
				if (chan is null)
					return TypeParserResult<DiscordChannel>.Unsuccessful("Failed to get a channel.");

				return TypeParserResult<DiscordChannel>.Successful(chan);
			}
			else return TypeParserResult<DiscordChannel>.Unsuccessful("Failed to get a valid channel ID.");
		}
	}
}
