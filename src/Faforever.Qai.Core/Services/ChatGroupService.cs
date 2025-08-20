using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using Faforever.Qai.Core.Commands.Context;
using Faforever.Qai.Core.Database;
using Faforever.Qai.Core.Database.Entities;
using Faforever.Qai.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Faforever.Qai.Core.Services
{
    public class ChatGroupService(QAIDatabaseModel database, ILogger<ChatGroupService> logger) : IChatGroupService
    {
        private readonly QAIDatabaseModel _database = database;
        private readonly ILogger<ChatGroupService> _logger = logger;

        public async Task<ChatGroupResult> CreateGameChatGroups(CurrentGameResult game, DiscordCommandContext context)
        {
            var result = new ChatGroupResult
            {
                Game = game,
                Success = false
            };

            try
            {
                // Resolve Discord IDs for all players
                await ResolvePlayerDiscordIds(game);

                // Create Discord voice channels
                result = await CreateDiscordVoiceChannels(game, context);

                result.Success = true;
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating chat groups for game {GameId}", game.GameId);
                result.ErrorMessage = $"Failed to create chat groups: {ex.Message}";
                return result;
            }
        }

        private async Task ResolvePlayerDiscordIds(CurrentGameResult game)
        {
            var playerFafIds = game.Players.Select(p => p.FafId).ToList();
            
            var accountLinks = await _database.AccountLinks
                .Where(link => playerFafIds.Contains(link.FafId))
                .ToListAsync();

            foreach (var player in game.Players)
            {
                var link = accountLinks.FirstOrDefault(l => l.FafId == player.FafId);
                if (link != null)
                {
                    player.DiscordId = link.DiscordId;
                }
            }
        }

        private async Task<ChatGroupResult> CreateDiscordVoiceChannels(CurrentGameResult game, DiscordCommandContext context)
        {
            var result = new ChatGroupResult { Game = game };

            // Check if user is in a voice channel
            var member = await context.Guild.GetMemberAsync(context.User.Id);
            if (member.VoiceState?.Channel == null)
            {
                result.ErrorMessage = "You must be in a voice channel to sort players into teams.";
                return result;
            }

            var activeChannel = member.VoiceState.Channel;
            var gameName = TruncateGameName(game.GameName);

            // Create voice channels for each actual team (exclude observers)
            var actualTeamPlayers = game.GetActualTeamPlayers();
            if (!actualTeamPlayers.Any())
            {
                result.ErrorMessage = "No team players found in the game (only observers).";
                return result;
            }

            var channelTasks = new List<Task<CreatedChannel?>>();
            
            for (int teamNum = 1; teamNum <= game.TeamCount; teamNum++)
            {
                channelTasks.Add(CreateTeamVoiceChannel(context.Guild, activeChannel, gameName, teamNum));
            }

            var createdChannels = await Task.WhenAll(channelTasks);
            result.CreatedChannels = createdChannels.Where(c => c != null).Cast<CreatedChannel>().ToList();

            // Move players to appropriate channels
            await MovePlayersToChannels(game, activeChannel, result.CreatedChannels);

            // Track unresolved players
            result.UnresolvedPlayers = game.Players
                .Where(p => p.DiscordId == null)
                .Select(p => p.Username)
                .ToList();

            return result;
        }

        private async Task<CreatedChannel?> CreateTeamVoiceChannel(DiscordGuild guild, DiscordChannel activeChannel, string gameName, int teamNum)
        {
            var channelName = $"Team {teamNum} - {gameName} (temp)";
            
            try
            {
                // Check if channel already exists
                var existingChannel = guild.Channels.Where(c => c.Value.Type == DSharpPlus.ChannelType.Voice && c.Value.Name == channelName).FirstOrDefault();
                if (existingChannel.Value != null)
                {
                    _logger.LogInformation("Voice channel {ChannelName} already exists", channelName);
                    return new CreatedChannel
                    {
                        TeamNumber = teamNum,
                        ChannelName = channelName,
                        Platform = "Discord",
                        DiscordChannelId = existingChannel.Value.Id
                    };
                }

                var channel = await guild.CreateVoiceChannelAsync(
                    channelName,
                    parent: activeChannel.Parent,
                    reason: $"Temporary channel for Team {teamNum} in FAF game {gameName}"
                );

                _logger.LogInformation("Created voice channel {ChannelName} for team {TeamNum}", channelName, teamNum);

                return new CreatedChannel
                {
                    TeamNumber = teamNum,
                    ChannelName = channelName,
                    Platform = "Discord",
                    DiscordChannelId = channel.Id
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create voice channel for team {TeamNum}", teamNum);
                return null;
            }
        }

        private async Task MovePlayersToChannels(CurrentGameResult game, DiscordChannel activeChannel, List<CreatedChannel> channels)
        {
            var channelMap = channels.ToDictionary(c => c.TeamNumber, c => c);
            var moveTasks = new List<Task>();

            // Get members currently in the voice channel via voice states
            var membersInChannel = activeChannel.Guild.VoiceStates.Values
                .Where(vs => vs.Channel?.Id == activeChannel.Id)
                .Select(vs => vs.User);

            foreach (var user in membersInChannel)
            {
                // Get the member object from the user
                if (activeChannel.Guild.Members.TryGetValue(user.Id, out var member))
                {
                    var gamePlayer = game.Players.FirstOrDefault(p => p.DiscordId == member.Id);
                    // Only move players who are in actual teams (not observers with team -1)
                    if (gamePlayer != null && gamePlayer.Team > 0 && channelMap.TryGetValue(gamePlayer.Team, out var targetChannelInfo))
                    {
                        if (targetChannelInfo.DiscordChannelId.HasValue)
                        {
                            var targetChannel = activeChannel.Guild.GetChannel(targetChannelInfo.DiscordChannelId.Value);
                            if (targetChannel is DiscordChannel voiceChannel)
                            {
                                moveTasks.Add(MovePlayerToChannel(member, voiceChannel, targetChannelInfo));
                            }
                        }
                    }
                }
            }

            await Task.WhenAll(moveTasks);
        }

        private async Task MovePlayerToChannel(DiscordMember member, DiscordChannel channel, CreatedChannel channelInfo)
        {
            try
            {
                await member.ModifyAsync(m => m.VoiceChannel = channel);
                channelInfo.MovedPlayers.Add(member.DisplayName);
                _logger.LogInformation("Moved {PlayerName} to {ChannelName}", member.DisplayName, channel.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to move {PlayerName} to {ChannelName}", member.DisplayName, channel.Name);
            }
        }


        private static string TruncateGameName(string gameName)
        {
            // Discord voice channels have a 100 character limit
            // Account for "Team X - " (9 chars) and " (temp)" (7 chars) = 16 chars
            const int maxGameNameLength = 100 - 16;
            return gameName.Length > maxGameNameLength 
                ? gameName[..maxGameNameLength] 
                : gameName;
        }
    }
}