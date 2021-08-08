using System;
using System.Linq;
using System.Threading.Tasks;
using Faforever.Qai.Core.Commands.Authorization;
using IrcDotNet;
using Qmmands;

namespace Faforever.Qai.Core.Commands.Context
{
    public class IrcCommandContext : CustomCommandContext
    {
        public string RespondTo { get; }
        public IrcUser Author { get; set; }
        public IrcClient Client { get; }
        public IrcLocalUser LocalUser { get;}
        public IrcChannel? Channel { get; }
        public IrcChannelUser? ChannelUser { get; }
        public string Message { get; }

        protected override bool isPrivate => Channel is null;

        public IrcCommandContext(IrcClient client, string respondTo, IrcUser author, string message, string prefix, IServiceProvider services, IrcChannel? channel = null) : base(services)
        {
            Client = client;
            LocalUser = client.LocalUser;
            RespondTo = respondTo;
            Message = message;
            Prefix = prefix;
            Author = author;
            Channel = channel;

            if (!(Channel is null))
                ChannelUser = Channel.GetChannelUser(Author);
            else ChannelUser = null;
        }

        protected override Task SendReplyAsync(object message, bool inPrivate = false)
        {
            return Task.Run(() =>
            {
                //IRC doesn't support newlines. So we replace those with spaces.
                var msg = message.ToString()?.Replace("\n", " ");
    
                if (inPrivate)
                    LocalUser.SendMessage(RespondTo, msg);
                else
                    LocalUser.SendMessage(Channel, msg);
            });
        }

        public override Task SendActionAsync(string action)
        {
            return Task.Run(() =>
            {
                action = action.Replace("\n", " ");
                IIrcMessageTarget? target = Channel;
                if (target is null)
                    target = Author;


                var actionMessage = IrcUtils.ActionMessage(action);
                LocalUser.SendMessage(target, actionMessage);
            });
        }

        // Permissions
        public async override Task<bool> CheckPermissionsAsync(CommandRequirements required)
        {
            var missing = GetMissingIrcRequirements(this, this.Author, required.Irc.User);
            if(missing != IrcPermissions.None)
            {
                await ReplyAsync("You do not have permission to execute that command!", ReplyOption.InPrivate);
                return false;
            }

            missing = GetMissingIrcRequirements(this, this.Client.LocalUser, required.Irc.User);
            if (missing != IrcPermissions.None)
            {
                await ReplyAsync("Bot lacks the permissions to execute that command!", ReplyOption.InPrivate);
                return false;
            }

            return true;
        }

        private static IrcPermissions GetMissingIrcRequirements(IrcCommandContext ircContext, IrcUser ircUser, IrcPermissions required)
        {
            // IRC operators always have permission
            if (ircUser.IsOperator)
                return IrcPermissions.None;

            var missing = IrcPermissions.None;

            if (required.HasFlag(IrcPermissions.ChannelModerator))
            {
                var channelUser = ircUser.GetChannelUsers().FirstOrDefault(cu => cu.Channel == ircContext.Channel && cu.User == ircUser);
                if (channelUser?.Modes.Contains('o') != true)
                    missing |= IrcPermissions.ChannelModerator;
            }

            if (required.HasFlag(IrcPermissions.AeolusModerator))
            {
                var aeolus = ircContext.Client.Channels.FirstOrDefault(c => c.Name.Equals("#aeolus", StringComparison.OrdinalIgnoreCase));
                var channelUser = aeolus?.GetChannelUser(ircUser);

                if (!IsChannelOp(channelUser))
                    missing |= IrcPermissions.AeolusModerator;
            }

            return missing;
        }

        private static bool IsChannelOp(IrcChannelUser? channelUser) => channelUser != null && channelUser.Modes.Contains('o');
    }
}
