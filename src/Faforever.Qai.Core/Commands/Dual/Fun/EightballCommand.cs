using System.Threading.Tasks;

using Faforever.Qai.Core.Services.BotFun;

using Qmmands;

namespace Faforever.Qai.Core.Commands.Dual.Fun
{
    public class EightballCommand(IBotFunService botFun) : DualCommandModule
    {
        [Command("eightball", "8ball")]
        [Description("Ask the mysterious 8ball a question.")]
        public async Task EightballCommandAsync()
        {
            var response = botFun.GetRandomEightballResponse();
            await Context.ReplyAsync(response);
        }
    }
}
