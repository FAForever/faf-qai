using System.Collections.Generic;
using System.Linq;
using System.Text;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.CommandsNext.Entities;
using DSharpPlus.Entities;

namespace Faforever.Qai.Discord.Commands
{
    // TODO: This is a pretty basic help formatter and could do with editing.

    /// <summary>
    /// This is currently the same as the DefaultHelpFormatter from DSharpPlus. Modify this class to edit your help formatting.
    /// </summary>
    public class HelpFormatter : BaseHelpFormatter
    {
        public DiscordEmbedBuilder EmbedBuilder { get; }
        private Command? Command { get; set; }

        public HelpFormatter(CommandContext ctx) : base(ctx)
        {
            EmbedBuilder = new DiscordEmbedBuilder()
                .WithTitle("Help")
                .WithColor(0x00ff95);
        }

        public override CommandHelpMessage Build()
        {
            if (this.Command is null)
                this.EmbedBuilder.WithDescription("Listing all modules. Specify a command to see more infomration.");

            return new CommandHelpMessage(embed: this.EmbedBuilder.Build());
        }

        public override BaseHelpFormatter WithCommand(Command command)
        {
            this.Command = command;

            this.EmbedBuilder.WithDescription($"{Formatter.InlineCode(command.Name)}: {command.Description ?? "No description provided."}");

            if (command is CommandGroup cgroup && cgroup.IsExecutableWithoutSubcommands)
                this.EmbedBuilder.WithDescription($"{this.EmbedBuilder.Description}\n\nThis group can be executed as a standalone command.");

            if (command.Aliases?.Any() == true)
                this.EmbedBuilder.AddField("Aliases", string.Join(", ", command.Aliases.Select(Formatter.InlineCode)), false);

            if (command.Overloads?.Any() == true)
            {
                var sb = new StringBuilder();

                foreach (var ovl in command.Overloads.OrderByDescending(x => x.Priority))
                {
                    sb.Append('`').Append(command.QualifiedName);

                    foreach (var arg in ovl.Arguments)
                        sb.Append(arg.IsOptional || arg.IsCatchAll ? " [" : " <").Append(arg.Name).Append(arg.IsCatchAll ? "..." : "").Append(arg.IsOptional || arg.IsCatchAll ? ']' : '>');

                    sb.Append("`\n");

                    foreach (var arg in ovl.Arguments)
                        sb.Append('`').Append(arg.Name).Append(" (").Append(this.CommandsNext.GetUserFriendlyTypeName(arg.Type)).Append(")`: ").Append(arg.Description ?? "No description provided.").Append('\n');

                    sb.Append('\n');
                }

                this.EmbedBuilder.AddField("Arguments", sb.ToString().Trim(), false);
            }

            return this;
        }

        public override BaseHelpFormatter WithSubcommands(IEnumerable<Command> subcommands)
        {
            this.EmbedBuilder.AddField(this.Command != null ? "Subcommands" : "Commands", string.Join(", ", subcommands.Select(x => Formatter.InlineCode(x.Name))), false);

            return this;
        }
    }
}
