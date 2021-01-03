using System.Threading.Tasks;

using DSharpPlus.Entities;

using Faforever.Qai.Core.Commands.Context;
using Faforever.Qai.Core.Models;
using Faforever.Qai.Core.Operations.Units;

using Qmmands;

namespace Faforever.Qai.Core.Commands.Dual.Info
{
	public class UnitCommand : DualCommandModule
	{
		private readonly ISearchUnitDatabaseOperation _unitSearch;

		public UnitCommand(ISearchUnitDatabaseOperation unitSearch)
		{
			this._unitSearch = unitSearch;
		}

		[Command("unit", "searchunit")]
		[Description("Search the Unit Database for a unit.")]
		public async Task UnitCommandAsync(string search)
		{
			var result = await this._unitSearch.SearchUnitDatabase(search);

			if (Context is DiscordCommandContext disCtx)
				await RespondDiscord(disCtx, result);
			else if (Context is IRCCommandContext ircCtx)
				await RespondIRC(ircCtx, result);
		}

		private async Task RespondIRC(IRCCommandContext ctx, UnitDatabaseSerachResult? result)
		{
			if (result is not null)
			{
				var desc = result.GeneralData.UnitName is not null ? $@"""{result.GeneralData.UnitName}"" {result.Description}" : result.Description;
				await ctx.ReplyAsync($"[{result.GeneralData.FactionName} - {result.Id}] {desc}: {result.GetUnitDatabaseUrl()}");
			}
			else
			{
				await ctx.ReplyAsync("No unit found.");
			}
		}

		private async Task RespondDiscord(DiscordCommandContext ctx, UnitDatabaseSerachResult? result)
		{
			var embed = new DiscordEmbedBuilder();
			if (result is not null)
			{
				var desc = result.GeneralData.UnitName is not null ? $@"""{result.GeneralData.UnitName}"" {result.Description}" : result.Description;
				embed.WithAuthor(desc, result.GetUnitDatabaseUrl(), result.GetStrategicIconUrl())
					.WithFooter($"{result.GeneralData.FactionName} - {result.Id}")
					.WithThumbnail(result.GetUnitImageUrl())
					.WithTitle("Click here to open unitDB")
					.WithUrl(result.GetUnitDatabaseUrl())
					.WithColor(result.GetFactionColor());
			}
			else
			{
				embed.WithColor(DiscordColor.Gray)
					.WithDescription("Failed to find a unit.");
			}

			await ctx.Channel.SendMessageAsync(embed: embed);
		}
	}
}
