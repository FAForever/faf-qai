using System.Threading.Tasks;
using Faforever.Qai.Core.Commands.Context;
using Faforever.Qai.Core.Models;

namespace Faforever.Qai.Core.Services
{
    public interface IChatGroupService
    {
        Task<ChatGroupResult> CreateGameChatGroups(CurrentGameResult game, DiscordCommandContext context);
    }
}