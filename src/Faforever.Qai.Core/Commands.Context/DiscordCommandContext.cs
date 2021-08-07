using System;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace Faforever.Qai.Core.Commands.Context
{
    public class DiscordCommandContext : CustomCommandContext
    {
        public DiscordChannel Channel
        {
            get
            {
                return Message.Channel;
            }
        }

        protected override bool isPrivate => false;

        public DiscordClient Client { get; private set; }
        public DiscordUser User { get; private set; }
        public DiscordMessage Message { get; private set; }
        public DiscordGuild Guild { get; private set; }

        public DiscordCommandContext(DiscordClient client, MessageCreateEventArgs args, string prefix, IServiceProvider services) : base(services)
        {
            Client = client;
            Prefix = prefix;
            Message = args.Message;
            Guild = args.Guild;
            User = args.Author;
        }

        protected override async Task SendReplyAsync(object message, bool inPrivate = false)
        {
            if (inPrivate)
            {
                var member = (DiscordMember)User;
                if (message is DiscordEmbed embed)
                    await member.SendMessageAsync(embed);
                else
                    await member.SendMessageAsync(message.ToString());
            }
            else
            {
                if (message is DiscordEmbed embed)
                    await Channel.SendMessageAsync(embed);
                else
                    await Channel.SendMessageAsync(message.ToString());
            }
        }

        public override Task SendActionAsync(string action)
        {
            //throw new NotImplementedException();

            return Task.CompletedTask;
        }
    }
}
