using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            try
            {
                var chartBytes = await getChartTask;
                
                var msgBuilder = new DiscordMessageBuilder()
                    .WithContent("Here is a chart of their rating history:")
                    .AddFile("chart.png", new MemoryStream(chartBytes));

                await msgBuilder.SendAsync(ctx.Channel);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("no rating history"))
            {
                await Context.ReplyAsync("This player has no rating history to display in a chart.");
            }
            catch (Exception)
            {
                await Context.ReplyAsync("Unable to generate rating chart due to an unexpected error.");
            }
        }

        [Command("playerstats")]
        public async Task GetDetailedPlayerStatsAsync(string username)
        {
            var playerStats = await _playerService.FetchDetailedPlayerStats(username);

            if (playerStats is null)
                await Context.ReplyAsync("No such player found.");
            else
                await ReplyWithDetailedStatsAsync(playerStats);
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

        private async Task ReplyWithDetailedStatsAsync(DetailedPlayerStatsResult data)
        {
            if (Context is DiscordCommandContext discordCtx)
                await DiscordDetailedStatsReplyAsync(discordCtx, data);
            else if (Context is IrcCommandContext ircCtx)
                await IrcDetailedStatsReplyAsync(ircCtx, data);
        }

        private async Task IrcDetailedStatsReplyAsync(IrcCommandContext ctx, DetailedPlayerStatsResult data)
        {
            var response = $"=== DETAILED STATS FOR {data.Name.ToUpper()} (Last {data.GameCountDisplay} Games) ===\n" +
                $"ID: {data.Id} | Last Seen: {data.LastSeen:yyyy-MM-dd HH:mm:ss}\n\n" +
                
                "== CURRENT RATINGS ==\n" +
                $"Global: {data.GlobalStats?.Rating.ToString("F0") ?? "Unranked"} (Rank #{data.GlobalStats?.Ranking ?? 0}, {data.GlobalStats?.GamesPlayed ?? 0} games)\n" +
                $"Ladder: {data.LadderStats?.Rating.ToString("F0") ?? "Unranked"} (Rank #{data.LadderStats?.Ranking ?? 0}, {data.LadderStats?.GamesPlayed ?? 0} games)\n\n" +

                "== PERFORMANCE HIGHLIGHTS ==\n" +
                $"Peak Global: {data.Performance.PeakGlobalRating:F0} ({data.Performance.PeakGlobalDate:yyyy-MM-dd})\n" +
                $"Peak Ladder: {data.Performance.PeakLadderRating:F0} ({data.Performance.PeakLadderDate:yyyy-MM-dd})\n" +
                $"Longest Win Streak: {data.Performance.LongestWinStreak} | Loss Streak: {data.Performance.LongestLossStreak}\n" +
                $"Most Played Map: {data.Performance.MostPlayedMap} ({data.Performance.MostPlayedMapCount} times)\n\n" +

                "== ACTIVITY OVERVIEW (Last {data.GameCountDisplay} Games) ==\n" +
                $"Total Games: {data.Activity.TotalGamesPlayed} | W/L: {data.Activity.TotalWins}/{data.Activity.TotalLosses} ({data.Activity.OverallWinRate:F1}%)\n" +
                $"Recent Activity: {data.Activity.GamesLast7Days} games (7d), {data.Activity.GamesLast30Days} games (30d)\n";

            if (data.FactionStatistics.Any())
            {
                response += "\n== FACTION PERFORMANCE ==\n";
                foreach (var faction in data.FactionStatistics.OrderByDescending(f => f.Value.GamesPlayed))
                {
                    var stats = faction.Value;
                    response += $"{faction.Key}: {stats.GamesPlayed} games, {stats.WinRate:F1}% WR, Avg Rating: {stats.AverageRating:F0}\n";
                }
            }

            if (data.MapStatistics.Any())
            {
                response += "\n== TOP MAPS ==\n";
                foreach (var map in data.MapStatistics.OrderByDescending(m => m.Value.GamesPlayed).Take(10))
                {
                    var stats = map.Value;
                    response += $"{map.Key}: {stats.GamesPlayed} games, {stats.WinRate:F1}% WR\n";
                }
            }

            await Context.ReplyAsync(response);
        }

        private async Task DiscordDetailedStatsReplyAsync(DiscordCommandContext ctx, DetailedPlayerStatsResult data)
        {
            var embed = new DiscordEmbedBuilder()
                .WithColor(Context.DostyaRed)
                .WithTitle($"📊 Detailed Stats for {data.Name} (Last {data.GameCountDisplay} Games)")
                .WithDescription($"**ID:** {data.Id} | **Last Seen:** {data.LastSeen:yyyy-MM-dd HH:mm}")
                .WithTimestamp(DateTime.UtcNow);

            // Basic ratings
            if (data.GlobalStats.HasValue || data.LadderStats.HasValue)
            {
                var ratingsText = "";
                if (data.GlobalStats.HasValue)
                    ratingsText += $"🌍 **Global:** {data.GlobalStats.Value.Rating:F0} (#{data.GlobalStats.Value.Ranking}, {data.GlobalStats.Value.GamesPlayed} games)\n";
                if (data.LadderStats.HasValue)
                    ratingsText += $"🎯 **Ladder:** {data.LadderStats.Value.Rating:F0} (#{data.LadderStats.Value.Ranking}, {data.LadderStats.Value.GamesPlayed} games)";
                
                embed.AddField("Current Ratings", ratingsText);
            }

            // Performance highlights
            var performanceText = $"🏆 **Peak Global:** {data.Performance.PeakGlobalRating:F0} ({data.Performance.PeakGlobalDate:MMM yyyy})\n" +
                $"🎖️ **Peak Ladder:** {data.Performance.PeakLadderRating:F0} ({data.Performance.PeakLadderDate:MMM yyyy})\n" +
                $"🔥 **Win Streak:** {data.Performance.LongestWinStreak} | 💀 **Loss Streak:** {data.Performance.LongestLossStreak}\n" +
                $"🗺️ **Favorite Map:** {data.Performance.MostPlayedMap} ({data.Performance.MostPlayedMapCount}×)\n" +
                $"⏱️ **Avg Game Time:** {data.Performance.AverageGameDuration:F0} min";
            embed.AddField("Performance", performanceText);

            // Activity overview
            var activityText = $"🎮 **Total Games:** {data.Activity.TotalGamesPlayed}\n" +
                $"📊 **W/L Ratio:** {data.Activity.TotalWins}/{data.Activity.TotalLosses} ({data.Activity.OverallWinRate:F1}%)\n" +
                $"📅 **Recent:** {data.Activity.GamesLast7Days} (7d), {data.Activity.GamesLast30Days} (30d)";
            embed.AddField("Activity", activityText);

            // Faction statistics (top 3)
            if (data.FactionStatistics.Any())
            {
                var topFactions = data.FactionStatistics.OrderByDescending(f => f.Value.GamesPlayed).Take(3);
                var factionText = string.Join("\n", topFactions.Select(f => 
                    $"**{f.Key}:** {f.Value.GamesPlayed} games ({f.Value.WinRate:F1}% WR)"));
                embed.AddField("Top Factions", factionText, true);
            }

            // Recent games (last 5)
            if (data.RecentGames.Any())
            {
                var recentText = string.Join("\n", data.RecentGames.Take(5).Select(g => 
                    $"{GetResultEmoji(g.Result)} **{g.MapName}** ({g.RatingChange:+0;-0}) - {g.Date:MM/dd}"));
                embed.AddField("Recent Games", recentText, true);
            }

            // Top maps (top 5)
            if (data.MapStatistics.Any())
            {
                var topMaps = data.MapStatistics.OrderByDescending(m => m.Value.GamesPlayed).Take(5);
                var mapsText = string.Join("\n", topMaps.Select(m => 
                    $"**{m.Key}:** {m.Value.GamesPlayed} games ({m.Value.WinRate:F1}% WR)"));
                embed.AddField("Top Maps", mapsText, true);
            }

            // Favorite opponents (top 3)
            if (data.Activity.FavoriteOpponents.Any())
            {
                var opponentsText = string.Join("\n", data.Activity.FavoriteOpponents.Take(3));
                embed.AddField("Frequent Opponents", opponentsText, true);
            }

            // Clan info
            if (data.Clan != null)
            {
                embed.AddField("Clan", $"[{data.Clan.Tag}] {data.Clan.Name}", true);
            }

            await Context.ReplyAsync(embed.Build());
        }

        private string GetResultEmoji(string result) => result switch
        {
            "Victory" => "🏆",
            "Defeat" => "💀", 
            "Draw" => "⚖️",
            _ => "❓"
        };
    }
}