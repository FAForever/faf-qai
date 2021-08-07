using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus.Entities;
using Faforever.Qai.Core.Operations.FafApi;
using Faforever.Qai.Core.Services;
using Qmmands;

namespace Faforever.Qai.Core.Commands.Dual.Replay
{
    public class FetchReplayCommand : DualCommandModule
    {
        private readonly GameService _gameService;

        public FetchReplayCommand(GameService gameService)
        {
            _gameService = gameService;
        }

        [Command("replay")]
        public async Task FetchReplayCommandAsync(long gameId)
        {
            var game = await _gameService.FetchGame(gameId);

            await RespondToUser(game);
        }

        [Command("lastreplay")]
        public async Task FetchLastReplayCommandAsync(string username)
        {
            var data = await _gameService.FetchLastGame(username);

            await RespondToUser(data);
        }

        [Command("topreplays")]
        public async Task FetchTopReplaysAsync(string mapName = null)
        {
            var games = await _gameService.FetchTopRatedGames(mapName, FafMod.Faf);

            foreach (var game in games)
                await RespondToUser(game);
        }

        [Command("top1v1replays")]
        public async Task FetchTopLadderReplaysAsync(string mapName = null)
        {
            var games = await _gameService.FetchTopRatedGames(mapName, FafMod.Ladder1v1);

            foreach (var game in games)
                await RespondToUser(game);
        }

        private async Task RespondToUser(Game? game)
        {
            if (game is null)
            {
                await ReplyAsync("Failed to get a replay by that ID");
                return;
            }

            await ReplyAsync(() => IrcResponse(game), () => DiscordResponse(game));
        }

        private DiscordEmbed DiscordResponse(Game game)
        {
            var embed = new DiscordEmbedBuilder();

            var description = new StringBuilder();

            var mapName = game.MapVersion != null ? $"{game.MapVersion.Map.DisplayName}" : "Unknown";

            embed
                .WithColor(Context.DostyaRed)
                .WithTitle($"{game.Name} - Download replay #{game.Id}")
                .WithThumbnail(game.MapVersion?.ThumbnailUrlLarge)
                .AddField("Map:", mapName, true)
                .AddField("Start Time", game.StartTime.ToString("u"), true)
                .AddField("Duration", game.GameDuration.ToString() ?? "-", true)
                .AddField("Avg rating", game.AverageRating().ToString() ?? "-", true)
                .WithUrl(game.ReplayUrl)
                ;

            var players = (game.PlayerStats ?? Enumerable.Empty<PlayerStats>()).OrderBy(s => s.StartSpot);
            var teams = new Dictionary<int, List<string>>();
            foreach (var p in players)
            {
                if (!teams.ContainsKey(p.Team))
                    teams[p.Team] = new();

                teams[p.Team].Add($"{p.Player.Login} ({p.BeforeRating}, {p.FactionName})");
            }

            var n = 1;
            foreach (var team in teams.OrderBy(t => t.Key))
            {
                var sb = new StringBuilder();

                foreach (var player in team.Value)
                    sb.Append($"{player}\n");

                embed.AddField($"Team {n}", sb.ToString(), true);
                n++;
            }

            embed.WithDescription(description.ToString());

            return embed;
        }

        public static string IrcResponse(Game game)
        {
            var mapName = game.MapVersion?.Map.DisplayName ?? "unknown map";
            var output = $"#{game.Id} {game.Name}, {mapName}, duration: {game.GameDuration}, rating: {game.AverageRating()}, {game.ReplayUrl}";

            return output;
        }
    }
}
