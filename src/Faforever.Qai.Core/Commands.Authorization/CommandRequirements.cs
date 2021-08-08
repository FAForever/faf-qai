using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;

namespace Faforever.Qai.Core.Commands.Authorization
{
    public class CommandRequirements
    {
        public DiscordCommandRequirements Discord { get; }
        public IrcCommandRequirements Irc { get; }

        public CommandRequirements()
        {
            this.Discord = new DiscordCommandRequirements();
            this.Irc = new IrcCommandRequirements();
        }

        public class DiscordCommandRequirements
        {
            public Permissions Bot { get; set; }
            public Permissions User { get; set; }
            public bool FafStaff { get; set; }
        }

        public class IrcCommandRequirements
        {
            public IrcPermissions Bot { get; set; }
            public IrcPermissions User { get; set; }
        }
    }
}
