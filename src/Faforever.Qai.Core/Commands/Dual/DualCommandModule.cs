
using System;
using System.Threading.Tasks;
using Faforever.Qai.Core.Commands.Context;

using Qmmands;

namespace Faforever.Qai.Core.Commands
{
    public class DualCommandModule<T> : DualCommandModule
    {
        // Use this for specific command tools, such as default formatting, standard complex responses, etc.

        public async Task ReplyAsync(T data)
        {
            if (Context is DiscordCommandContext dctx)
                await ReplyAsync(dctx, data);
            else if (Context is IrcCommandContext ictx)
                await ReplyAsync(ictx, data);
        }

        public virtual Task ReplyAsync(DiscordCommandContext ctx, T data) => ctx.Channel.SendMessageAsync(data?.ToString() ?? "No Data");
        public virtual Task ReplyAsync(IrcCommandContext ctx, T data) => ctx.ReplyAsync(data?.ToString() ?? "No Data");
    }

    public class DualCommandModule : ModuleBase<CustomCommandContext>
    {
        protected virtual Task ReplyAsync(Func<string> getIrcReply, Func<object>? getDiscordReply)
        {
            object message;

            if (Context is DiscordCommandContext && getDiscordReply != null)
                message = getDiscordReply();
            else
                message = getIrcReply();

            return Context.ReplyAsync(message);
        }

        protected virtual Task ReplyAsync(string message)
        {
            return Context.ReplyAsync(message);
        }
    }
}
