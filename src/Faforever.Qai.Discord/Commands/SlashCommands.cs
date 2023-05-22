#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
#pragma warning disable CA1822 // Mark members as static
#pragma warning disable IDE0060 // Remove unused parameter

using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Faforever.Qai.Core.Operations.PatchNotes;
using System.Threading.Tasks;

namespace Faforever.Qai.Discord.Commands
{
    public class SlashCommands : ApplicationCommandModule
    {
        private readonly DiscordBot bot;

        public SlashCommands(DiscordBot bot)
        {
            this.bot = bot;
        }

        [SlashCommand("ping", "Ping!")]
        public async Task PingCommand(InteractionContext ctx) { }

        [SlashCommand("player", "Get info about a player")]
        public async Task PlayerCommand(InteractionContext ctx, [Option("name", "Name of player")]string name) { }

        [SlashCommand("seen", "Find out when a player last was seen")]
        public async Task SeenPlayerCommand(InteractionContext ctx, [Option("name", "Name of player")] string name) { }

        [SlashCommand("searchplayer", "Search for a player")]
        public async Task SearchPlayerCommand(InteractionContext ctx,[Option("name", "Name of player")] string name) { }

        [SlashCommand("map", "Get a map from the map database.")]
        public async Task MapCommand (InteractionContext ctx, [Option("name", "Name of map")] string name) { }

        [SlashCommand("pool", "Show the current maps in the ladder pool")]
        public async Task PoolCommand(InteractionContext ctx) { }

        [SlashCommand("replay", "Get info about a replay")]
        public async Task ReplayCommand(InteractionContext ctx, [Option("gameId", "ID of game")] long gameId) { }

        [SlashCommand("lastreplay", "Get last replay of player")]
        public async Task LastReplayCommand(InteractionContext ctx, [Option("name", "Name of player")] string name) { }

        [SlashCommand("topreplays", "Get recent replays with high rated players")]
        public async Task TopReplaysCommand(InteractionContext ctx, [Option("mapName", "Map name")] string? mapName = null) { }

        [SlashCommand("top1v1replays", "Get recent 1v1 replays with high rated players")]
        public async Task Top1v1ReplaysCommand(InteractionContext ctx, [Option("mapName", "Map name")] string? mapName = null) { }

        [SlashCommand("clan", "Get info about a clan")]
        public async Task ClanCommand(InteractionContext ctx, [Option("name", "Name of clan")] string name) { }

        [SlashCommand("patchnotes", "Get link to patch notes")]
        public async Task PatchNotesCommand(InteractionContext ctx, [Option("version", "Version number")][Autocomplete(typeof(FetchPatchNotesLinkOperation))] string? version = null) { }

        [SlashCommand("unit", "Search the Unit Database for a unit")]
        public async Task UnitCommand(InteractionContext ctx, [Option("name", "Name of the unit")] string name) {}

        [SlashCommand("url", "Search for a specific url")]
        public async Task UrlCommand(InteractionContext ctx, [Option("name", "Name of the unit")] string contains) { }

        [SlashCommand("wiki", "Search for a specific wiki url")]
        public async Task WikiCommand(InteractionContext ctx, [Option("name", "Name of the unit")] string contains) { }

        /*
        [ContextMenu(ApplicationCommandType.UserContextMenu, "User Menu")]
        public async Task UserMenu(ContextMenuContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Hello context!"));
        }
        */
    }

    // Used to unregister slash commands
    public class EmptySlashCommands : ApplicationCommandModule { }

}

#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore CA1822 // Mark members as static
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously