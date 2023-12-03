using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using DSharpPlus.Entities;

using Faforever.Qai.Core.Commands.Context;
using Faforever.Qai.Core.Constants;
using Faforever.Qai.Core.Models;
using Faforever.Qai.Core.Services;

using Qmmands;

namespace Faforever.Qai.Core.Commands.Dual.Player
{
    public class PlayerCommands : DualCommandModule<FetchPlayerStatsResult>
    {
        private readonly IPlayerService _playerService;

        public PlayerCommands(IPlayerService playerService)
        {
            _playerService = playerService;
        }

        [Command("player", "ratings")]
        public async Task GetRatingsAsync(string username)
        {
            FetchPlayerStatsResult? playerStats = await _playerService.FetchPlayerStats(username);

            if (playerStats is null)
                await Context.ReplyAsync("No such player found.");
            else await ReplyAsync(playerStats);

        }

        public override Task IrcReplyAsync(IrcCommandContext ctx, FetchPlayerStatsResult data)
        {
            return Context.ReplyAsync($"found player '{data.Name}' with the following information:\n" +
                    $"1v1: rating '{data.LadderStats?.Rating.ToString("F0") ?? "0"}', ranked '{data.LadderStats?.Ranking ?? 0}'\n" +
                    $"Global: rating '{data.GlobalStats?.Rating.ToString("F0") ?? "0"}', ranked '{data.GlobalStats?.Ranking ?? 0}'");
        }

        public override async Task DiscordReplyAsync(DiscordCommandContext ctx, FetchPlayerStatsResult data)
        {
            var getChartTask = _playerService.GenerateRatingChart(data.Name, FafLeaderboard.Global);

            List<string> toJoin;

            if (data.OldNames.Count > 5)
            {
                toJoin = data.OldNames.GetRange(0, 5);
                toJoin.Add("...");
            }
            else
            {
                toJoin = data.OldNames;
            }
            const string dateFormat = "yyyy-MM-dd HH:mm:ss";
            var embed = new DiscordEmbedBuilder()
                .WithColor(Context.DostyaRed)
                .WithTitle(data.Name)
                .WithDescription($"**ID: {data.Id}**, Last seen: {data.LastSeen.ToString(dateFormat)}")
                .WithColor(DiscordColor.Red);

            if (toJoin.Count != 0)
                embed.AddField("Aliases", string.Join("\n", toJoin));

            if (!(data.LadderStats is null))
                embed.AddField("Ladder:", "```http\n" +
                    $"Rating  :: {data.LadderStats?.Rating.ToString("F0") ?? "0"}\n" +
                    $"Ranking :: {data.LadderStats?.Ranking ?? 0}\n" +
                    $"Games   :: {data.LadderStats?.GamesPlayed ?? 0}\n" +
                    "```");

            if (!(data.GlobalStats is null))
                embed.AddField("Global:", "```http\n" +
                    $"Rating  :: {data.GlobalStats?.Rating.ToString("F0") ?? "0"}\n" +
                    $"Ranking :: {data.GlobalStats?.Ranking ?? 0}\n" +
                    $"Games   :: {data.GlobalStats?.GamesPlayed ?? 0}\n" +
                    "```");

            if (!(data.Clan is null))
                embed.AddField($"Clan: {data.Clan?.Name}", "```http\n" +
                    $"Clan Size :: {0}\n" +
                    $"URL       :: {data.Clan?.WebsiteUrl ?? "n/a"}\n" +
                    "```");

            await Context.ReplyAsync(embed.Build());

            var chartBytes = await getChartTask;
            
            var msgBuilder = new DiscordMessageBuilder()
                .WithContent("Here is a chart of their rating history:")
                .AddFile("chart.png", new MemoryStream(chartBytes));

            await msgBuilder.SendAsync(ctx.Channel);
        }

        [Command("searchplayer")]
        public async Task FindPlayerAsync(string searchTerm)
        {
            FindPlayerResult findPlayerResult = await _playerService.FindPlayer(searchTerm);
            if (findPlayerResult.Usernames.Count == 0)
                await Context.ReplyAsync($"Found no players when searching for '{searchTerm}'");
            else
            {
                string players = string.Join(", ", findPlayerResult.Usernames);
                await Context.ReplyAsync($"Found the following players: {players}");
            }
        }
    }
}