
using System;
using System.Threading.Tasks;

using Faforever.Qai.Core.Commands.Context;

using Qmmands;

namespace Faforever.Qai.Core.Commands
{
    public class IrcCommandModule  : ModuleBase<IrcCommandContext>
    {
        /// <summary>
        /// Responds with a basic success message.
        /// </summary>
        /// <param name="message">Text to send.</param>
        /// <returns></returns>
        protected async Task RespondBasicSuccess(string message)
        {
            if (this.Context is null)
                throw new NullReferenceException("Command Context is null");

            await this.Context.ReplyAsync(message);
        }

        /// <summary>
        /// Responds with a basic error message.
        /// </summary>
        /// <param name="message">Text to send.</param>
        /// <returns></returns>
        protected async Task RespondBasicError(string message)
        {
            if (this.Context is null)
                throw new NullReferenceException("Command Conntext is null");

            await this.Context.ReplyAsync(message);
        }
    }
}
