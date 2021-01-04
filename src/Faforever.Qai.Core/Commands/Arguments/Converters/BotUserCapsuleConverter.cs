using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Faforever.Qai.Core.Commands.Context;

using IrcDotNet;

using Qmmands;

namespace Faforever.Qai.Core.Commands.Arguments.Converters
{
	public class BotUserCapsuleConverter : TypeParser<IBotUserCapsule>
	{
		public override async ValueTask<TypeParserResult<IBotUserCapsule>> ParseAsync(Parameter parameter, string value, CommandContext context)
		{
			if(context is IRCCommandContext ircCtx)
			{
				var cap = GetIrcUserCapsule(parameter, value, ircCtx);
				if(cap is not null)
					return TypeParserResult<IBotUserCapsule>.Successful(cap);
			}
			else if (context is DiscordCommandContext disCtx)
			{
				var cap = await GetDiscordUserOrMemberCapsule(parameter, value, disCtx);
				if(cap is not null)
					return TypeParserResult<IBotUserCapsule>.Successful(cap);
			}

			return TypeParserResult<IBotUserCapsule>.Unsuccessful("Failed to get a valid user.");
		}

		private IrcUserCapsule? GetIrcUserCapsule(Parameter p, string v, IRCCommandContext ctx)
		{
			// TODO: Improve this?
			var users = ctx.Client.GetChannelUsers();
			var user = users.FirstOrDefault(x => x.User.NickName == v);
			if (user is not null)
				return new IrcUserCapsule(user.User);

			return null;
		}

		private async Task<IBotUserCapsule?> GetDiscordUserOrMemberCapsule(Parameter p, string v, DiscordCommandContext ctx)
		{
			var valToParse = v;

			if (valToParse.StartsWith("<@"))
				valToParse = valToParse[2..(valToParse.Length - 1)];

			if (valToParse.StartsWith("<!@"))
				valToParse = valToParse[3..(valToParse.Length - 1)];

			if (valToParse.EndsWith(">"))
				valToParse = valToParse[..(valToParse.Length - 2)];

			if(ulong.TryParse(valToParse, out var id))
			{
				var member = await ctx.Guild.GetMemberAsync(id);

				if (member is not null)
					return new DiscordMemberCapsule(member);

				var user = await ctx.Client.GetUserAsync(id);

				if (user is not null)
					return new DiscordUserCapsule(user);
			}

			return null;
		}
	}
}
