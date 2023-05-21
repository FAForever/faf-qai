using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Faforever.Qai.Core.Commands.Authorization;
using Faforever.Qai.Discord.Core.Structures.Configurations;

namespace Faforever.Qai.Core.Commands.Context
{
    public class DiscordCommandContext : CustomCommandContext
    {
        public DiscordChannel Channel { get; private set; }
        public DiscordBotConfiguration Config { get; }
        public DiscordClient Client { get; private set; }
        public DiscordUser User { get; private set; }
        public DiscordInteraction? Interaction { get; }
        public DiscordGuild Guild { get; }

        protected override bool isPrivate => Channel.IsPrivate;

        public DiscordCommandContext(DiscordClient client, MessageCreateEventArgs args, DiscordBotConfiguration discordConfig, IServiceProvider services) : base(services)
        {
            Config = discordConfig;
            Client = client;
            Prefix = discordConfig.Prefix;
            Channel = args.Message.Channel;
            Guild = args.Guild;
            User = args.Author;
        }

        public DiscordCommandContext(DiscordClient client, DiscordInteraction interaction, DiscordBotConfiguration discordConfig, IServiceProvider services) : base(services)
        {
            Config = discordConfig;
            Client = client;
            Prefix = discordConfig.Prefix;
            Channel = interaction.Channel;
            Guild = interaction.Guild;
            User = interaction.User;
            Interaction = interaction;
        }

        protected override async Task SendReplyAsync(object message, bool inPrivate = false)
        {
            if (Interaction != null)
            {
                DiscordEmbed? embed = message as DiscordEmbed;
                embed ??= (message as DiscordEmbedBuilder)?.Build();
                
                if (embed != null)
                    await Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));
                else
                    await Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent(message.ToString()));
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
                if(Guild is null)
                {
                    await ReplyAsync("Command needs to be run in a channel!", ReplyOption.InPrivate);
                    return false;
                }

                var selfMember = await Guild.GetMemberAsync(Client.CurrentUser.Id);
                if(!Channel.PermissionsFor(selfMember).HasPermission(perms.Bot))
                {
                    await ReplyAsync("Bot do not have permission to execute that command!", ReplyOption.InPrivate);
                    return false;
                }
            }

            if(perms.FafStaff)
            {
                var fafStaff = Config.FafStaff ?? Array.Empty<ulong>();
                
                if (!fafStaff.Contains(User.Id))
                    return false;
            }

            return true;
        }
    }
}
