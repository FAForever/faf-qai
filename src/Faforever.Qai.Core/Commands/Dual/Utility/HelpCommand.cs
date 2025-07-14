using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using Faforever.Qai.Core.Commands.Context;
using Faforever.Qai.Core.Models;
using Faforever.Qai.Core.Services;
using Qmmands;

namespace Faforever.Qai.Core.Commands.Dual.Utility
{
    public class HelpCommand : DualCommandModule<List<CommandCategory>>
    {
        private readonly ICommandDiscoveryService _commandDiscovery;

        public HelpCommand(ICommandDiscoveryService commandDiscovery)
        {
            _commandDiscovery = commandDiscovery;
        }

        [Command("help", "commands")]
        [Description("Shows available commands or help for a specific command/category.")]
        public async Task HelpCommandAsync([Remainder] string? query = null)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                // Show all commands grouped by category
                var allCommands = _commandDiscovery.GetAvailableCommands(Context);
                await ReplyAsync(allCommands);
            }
            else
            {
                // Try to find specific command first
                var specificCommand = _commandDiscovery.GetCommand(query);
                if (specificCommand != null)
                {
                    await ReplySpecificCommand(specificCommand);
                    return;
                }

                // Try to find category
                var categoryCommands = _commandDiscovery.GetCommandsByCategory(query);
                if (categoryCommands.Any())
                {
                    var category = new CommandCategory 
                    { 
                        Name = query, 
                        Commands = categoryCommands 
                    };
                    await ReplyAsync(new List<CommandCategory> { category });
                    return;
                }

                await Context.ReplyAsync($"No command or category found for '{query}'");
            }
        }

        private async Task ReplySpecificCommand(CommandInfo command)
        {
            if (Context is DiscordCommandContext)
            {
                var embed = new DiscordEmbedBuilder()
                    .WithTitle($"Command: {command.Name}")
                    .WithColor(Context.DostyaRed)
                    .WithDescription(command.Description)
                    .AddField("Usage", command.Usage, false)
                    .AddField("Category", command.Category, true);

                if (command.Aliases.Any())
                {
                    embed.AddField("Aliases", string.Join(", ", command.Aliases), true);
                }

                await Context.ReplyAsync(embed);
            }
            else
            {
                var response = $"Command: {command.Name} - {command.Description}\n" +
                              $"Usage: {command.Usage}\n" +
                              $"Category: {command.Category}";
                
                if (command.Aliases.Any())
                {
                    response += $"\nAliases: {string.Join(", ", command.Aliases)}";
                }

                await Context.ReplyAsync(response);
            }
        }

        public override async Task DiscordReplyAsync(DiscordCommandContext ctx, List<CommandCategory> data)
        {
            var embed = new DiscordEmbedBuilder()
                .WithTitle("Available Commands")
                .WithColor(Context.DostyaRed)
                .WithDescription("Use `!help <command>` for detailed help on a specific command.\nUse `!help <category>` to see commands in a category.");

            foreach (var category in data.Take(10)) // Limit to prevent embed size issues
            {
                var commandList = category.Commands
                    .Take(10) // Limit commands per category
                    .Select(c => $"`{c.Name}` - {c.Description}")
                    .ToList();

                if (commandList.Count == 0)
                    continue;

                if (category.Commands.Count > 10)
                {
                    commandList.Add($"... and {category.Commands.Count - 10} more");
                }

                var fieldValue = string.Join("\n", commandList);
                if (fieldValue.Length > 1024) // Discord field value limit
                {
                    fieldValue = fieldValue.Substring(0, 1020) + "...";
                }

                embed.AddField($"{category.Name} Commands", fieldValue, false);
            }

            if (data.Count > 10)
            {
                embed.WithFooter($"Showing 10 of {data.Count} categories. Use !help <category> for more details.");
            }

            await ctx.ReplyAsync(embed);
        }

        public override async Task IrcReplyAsync(IrcCommandContext ctx, List<CommandCategory> data)
        {
            var response = new StringBuilder();
            response.AppendLine("Available Commands:");
            response.AppendLine("Use !help <command> for detailed help. Use !help <category> for category commands.");
            response.AppendLine();

            foreach (var category in data.Take(5)) // Limit for IRC
            {
                response.AppendLine($"[{category.Name}]");
                var commands = category.Commands.Take(8).Select(c => c.Name);
                response.AppendLine($"  {string.Join(", ", commands)}");
                
                if (category.Commands.Count > 8)
                {
                    response.AppendLine($"  ... and {category.Commands.Count - 8} more");
                }
                response.AppendLine();
            }

            if (data.Count > 5)
            {
                response.AppendLine($"Showing 5 of {data.Count} categories. Use !help <category> for more details.");
            }

            await ctx.ReplyAsync(response.ToString());
        }
    }
}
