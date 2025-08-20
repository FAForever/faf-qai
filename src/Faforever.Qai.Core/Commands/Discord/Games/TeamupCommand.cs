using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using Faforever.Qai.Core.Database;
using Faforever.Qai.Core.Database.Entities;
using Faforever.Qai.Core.Models;
using Faforever.Qai.Core.Operations.Games;
using Faforever.Qai.Core.Services;
using Qmmands;

namespace Faforever.Qai.Core.Commands.Discord.Games
{
    public class TeamupCommand : DiscordCommandModule
    {
        private readonly IFetchCurrentGameOperation _fetchCurrentGameOperation;
        private readonly IChatGroupService _chatGroupService;
        private readonly QAIDatabaseModel _database;

        public TeamupCommand(
            IFetchCurrentGameOperation fetchCurrentGameOperation,
            IChatGroupService chatGroupService,
            QAIDatabaseModel database)
        {
            _fetchCurrentGameOperation = fetchCurrentGameOperation;
            _chatGroupService = chatGroupService;
            _database = database;
        }

        [Command("teamup", "sort")]
        [Description("Create voice channels for your current FAF game teams")]
        public async Task TeamupAsync(DiscordUser? targetUser = null)
        {
            // Determine which user to check for current game
            ulong targetDiscordId;
            string targetUsername;

            if (targetUser != null)
            {
                targetDiscordId = targetUser.Id;
                targetUsername = targetUser.Username;
                
                // Check permissions for sorting other users' games
                if (!await HasPermissionToSortForOthers())
                {
                    await RespondBasicError("You don't have permission to sort games for other users.");
                    return;
                }
            }
            else
            {
                targetDiscordId = Context.User.Id;
                targetUsername = Context.User.Username;
            }

            // Get FAF username from account link
            var accountLink = await _database.FindAsync<AccountLink>(targetDiscordId);
            if (accountLink == null)
            {
                var message = targetUser != null 
                    ? $"User {targetUsername} is not linked to a FAF account. They need to use `!link` first."
                    : "You are not linked to a FAF account. Use `!link` to connect your Discord and FAF accounts.";
                
                await RespondBasicError(message);
                return;
            }

            // Fetch current game
            var currentGame = await _fetchCurrentGameOperation.FetchCurrentGameById(accountLink.FafId);
            if (currentGame == null)
            {
                var playerName = targetUser?.Username ?? "you";
                await RespondBasicError($"No active game found for {(targetUser != null ? playerName : "you")}. Make sure you're currently in a FAF game.");
                return;
            }

            // Validate game is actually current (started recently and no end time)
            if (!currentGame.IsActive)
            {
                await RespondBasicError("Your last game appears to have ended. This command only works for active games.");
                return;
            }

            // Create chat groups
            var result = await _chatGroupService.CreateGameChatGroups(currentGame, Context);
            
            if (!result.Success)
            {
                await RespondBasicError($"Failed to create chat groups: {result.ErrorMessage}");
                return;
            }

            // Send success response
            await SendSuccessResponse(result);
        }

        private async Task SendSuccessResponse(ChatGroupResult data)
        {
            var embed = SuccessBase()
                .WithTitle($"🎮 Team Up: {data.Game.GameName}")
                .WithDescription($"Hosted by **{data.Game.HostUsername}**")
                .AddField("Game Started", data.Game.StartTime.ToString("yyyy-MM-dd HH:mm:ss UTC"), true)
                .AddField("Total Players", data.Game.Players.Count.ToString(), true)
                .AddField("Teams", data.Game.TeamCount.ToString(), true);

            if (data.CreatedChannels.Any())
            {
                var channelList = string.Join("\n", data.CreatedChannels.Select(c => 
                    $"🔊 **{c.ChannelName}** - {c.MovedPlayers.Count} players moved"));
                embed.AddField("Created Channels", channelList);
            }

            if (data.UnresolvedPlayers.Any())
            {
                var unresolvedList = string.Join(", ", data.UnresolvedPlayers.Take(10));
                if (data.UnresolvedPlayers.Count > 10)
                    unresolvedList += $" (and {data.UnresolvedPlayers.Count - 10} more)";
                
                embed.AddField("⚠️ Unlinked Players", 
                    $"{unresolvedList}\n*These players need to use `!link` to connect their Discord accounts.*");
            }

            var observers = data.Game.GetObservers();
            if (observers.Any())
            {
                var observerNames = string.Join(", ", observers.Select(o => o.Username));
                embed.AddField("👁️ Observers", $"{observerNames}\n*Observers are not sorted into team channels.*");
            }

            await Context.Channel.SendMessageAsync(embed: embed.Build());
        }

        private async Task<bool> HasPermissionToSortForOthers()
        {
            try
            {
                var member = await Context.Guild.GetMemberAsync(Context.User.Id);
                // Check for specific roles or permissions that allow sorting for others
                return member.Permissions.HasFlag(DSharpPlus.Permissions.ManageChannels) ||
                       member.Roles.Any(r => r.Name.Contains("Moderator", System.StringComparison.OrdinalIgnoreCase) || 
                                            r.Name.Contains("Admin", System.StringComparison.OrdinalIgnoreCase));
            }
            catch
            {
                return false;
            }
        }
    }
}