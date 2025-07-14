using System.Collections.Generic;
using System.Threading.Tasks;
using Faforever.Qai.Core.Commands.Context;
using Faforever.Qai.Core.Models;

namespace Faforever.Qai.Core.Services
{
    public interface ICommandDiscoveryService
    {
        List<CommandCategory> GetAllCommands();
        List<CommandInfo> GetCommandsByCategory(string category);
        CommandInfo? GetCommand(string commandName);
        List<CommandCategory> GetAvailableCommands(CustomCommandContext context);
    }
}
