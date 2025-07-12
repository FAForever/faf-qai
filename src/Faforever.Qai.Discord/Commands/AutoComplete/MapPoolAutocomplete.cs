using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Faforever.Qai.Core;
using Faforever.Qai.Core.Operations.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faforever.Qai.Discord.Commands.AutoComplete
{
    public class MapPoolAutocomplete : IAutocompleteProvider
    {
        private static readonly Debouncer<Tuple<ulong, ulong>> _debouncer = new Debouncer<Tuple<ulong, ulong>>(TimeSpan.FromMilliseconds(200));
        private IFetchLadderPoolOperation _ladderOp;

        public MapPoolAutocomplete(IFetchLadderPoolOperation ladderOp)
        {
            this._ladderOp = ladderOp;
        }

        public async Task<IEnumerable<DiscordAutoCompleteChoice>> Provider(AutocompleteContext ctx)
        {
            var interactionId = ctx.Interaction.Id;
            var userId = ctx.Interaction.User.Id;
            var key = Tuple.Create(userId, interactionId);

            var choices = await _debouncer.Debounce(key, () => FetchLadderPoolChoices(ctx.OptionValue?.ToString()));

            return choices ?? [];
        }

        private async Task<IEnumerable<DiscordAutoCompleteChoice>> FetchLadderPoolChoices(string? searchTerm)
        {
            var data = await _ladderOp.FetchLadderPoolAsync();

            if (data == null || !data.Any())
                return Array.Empty<DiscordAutoCompleteChoice>();

            var choices = data.Where(m => m.MapVersions.Any()).Select(m => new DiscordAutoCompleteChoice(m.MatchmakerQueueMapPool.Name, m.Id.ToString()));

            if (searchTerm != null)
                choices = choices.Where(c => c.Name.Contains(searchTerm, StringComparison.InvariantCultureIgnoreCase));

            return choices;
        }
    }
}
