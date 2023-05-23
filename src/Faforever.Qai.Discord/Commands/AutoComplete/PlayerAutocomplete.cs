using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Faforever.Qai.Core;
using Faforever.Qai.Core.Operations.FafApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faforever.Qai.Discord.Commands.AutoComplete
{
    public class PlayerAutocomplete : IAutocompleteProvider
    {
        private readonly Debouncer<Tuple<ulong, ulong>> _debouncer = new Debouncer<Tuple<ulong, ulong>>(TimeSpan.FromMilliseconds(300));
        private FafApiClient _api;

        public PlayerAutocomplete(FafApiClient api)
        {
            this._api = api;
        }

        public async Task<IEnumerable<DiscordAutoCompleteChoice>> Provider(AutocompleteContext ctx)
        {
            var interactionId = ctx.Interaction.Id;
            var userId = ctx.Interaction.User.Id;
            var key = Tuple.Create(userId, interactionId);

            // debounce so only the last incomiing autocomplete request per user is processed

            var list = new List<DiscordAutoCompleteChoice>();
            var optionValue = ctx.OptionValue?.ToString();
            if (string.IsNullOrEmpty(optionValue))
                return list;

            var players = await _debouncer.Debounce(key, () => AutocompletePlayerNameAsync(optionValue));
            foreach (var player in players ?? Array.Empty<Player>())
                list.Add(new DiscordAutoCompleteChoice(player.Login, player.Login));

            return list;
        }

        private async Task<IEnumerable<Player>?> AutocompletePlayerNameAsync(string searchTerm, int limit = 20)
        {
            var query = new ApiQuery<Player>()
                .Fields("player", "login")
                .Where("login", $"{searchTerm}*")
                .Limit(limit);

            var players = await _api.GetAsync(query);

            return players;
        }
    }
}
