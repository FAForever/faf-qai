using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus.Entities;

using Faforever.Qai.Core.Commands.Context;

using Qmmands;

namespace Faforever.Qai.Discord.Commands
{
	public static class DiscordOnlyQmmandsTestCommand
	{
		public static async Task Execute(DiscordCommandContext ctx)
		{
			await ctx.ReplyAsync("Test basic response.");
			await ctx.Channel.SendMessageAsync(embed: new DiscordEmbedBuilder().WithDescription("Test Embed."));
		}
	}
}
