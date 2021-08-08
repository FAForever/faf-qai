using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Faforever.Qai.Core.Commands.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
            return Task.CompletedTask;
        }

        public async override Task<bool> CheckPermissionsAsync(CommandRequirements required)
        {
            var perms = required.Discord;

            if(perms.User != Permissions.None)
            {
                var member = (DiscordMember)User;
                if(!member.Permissions.HasPermission(perms.User))
                {
                    await ReplyAsync("You do not have permission to execute that command!", ReplyOption.InPrivate);
                    return false;
                }
            }

            if(perms.Bot != Permissions.None)
            {
                var selfMember = await Guild.GetMemberAsync(Client.CurrentUser.Id);
                if(!Channel.PermissionsFor(selfMember).HasPermission(perms.Bot))
                {
                    await ReplyAsync("Bot do not have permission to execute that command!", ReplyOption.InPrivate);
                    return false;
                }
            }

            if(perms.FafStaff)
            {
                var config = Services.GetService<IConfiguration>();
                HashSet<ulong> fafStaff = new(from child in config?.GetSection("Roles:FafStaff").GetChildren()
                                              where ulong.TryParse(child.Value, out _)
                                              select ulong.Parse(child.Value));

                if (!fafStaff.Contains(User.Id))
                    return false;
            }

            return true;
        }
    }
}
