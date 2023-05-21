using System.Threading.Tasks;

using DSharpPlus.Entities;

using Faforever.Qai.Core.Commands.Context;
using Faforever.Qai.Core.Models;
using Faforever.Qai.Core.Operations.Units;

using Qmmands;

namespace Faforever.Qai.Core.Commands.Dual.Info
{
    public class UnitCommand : DualCommandModule<UnitDatabaseSerachResult>
    {
        private readonly ISearchUnitDatabaseOperation _unitSearch;

        public UnitCommand(ISearchUnitDatabaseOperation unitSearch)
        {
            this._unitSearch = unitSearch;
        }

        [Command("unit", "searchunit")]
        [Description("Search the Unit Database for a unit.")]
        public async Task UnitCommandAsync([Remainder] string search)
        {
            var result = await this._unitSearch.SearchUnitDatabase(search);

            if (result is null)
                await Context.ReplyAsync("No unit found.");
            else await ReplyAsync(result);
        }

        public override async Task IrcReplyAsync(IrcCommandContext ctx, UnitDatabaseSerachResult data)
        {
            var desc = data.GeneralData.UnitName is not null ? $@"""{data.GeneralData.UnitName}"" {data.Description}" : data.Description;
            await ctx.ReplyAsync($"[{data.GeneralData.FactionName} - {data.Id}] {desc}: {data.GetUnitDatabaseUrl()}");
        }

        public override async Task DiscordReplyAsync(DiscordCommandContext ctx, UnitDatabaseSerachResult data)
        {
            var embed = new DiscordEmbedBuilder();
            var desc = data.GeneralData.UnitName is not null ? $@"""{data.GeneralData.UnitName}"" {data.Description}" : data.Description;
            embed.WithAuthor(desc, data.GetUnitDatabaseUrl(), data.GetStrategicIconUrl())
                .WithFooter($"{data.GeneralData.FactionName} - {data.Id}")
                .WithThumbnail(data.GetUnitImageUrl())
                .WithTitle("Click here to open unitDB")
                .WithUrl(data.GetUnitDatabaseUrl())
                .WithColor(data.GetFactionColor());

            await Context.ReplyAsync(embed);
        }
    }
}
