using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace Faforever.Qai.Discord.Commands.Data
{
	public class UnitCommand : CommandModule
	{
		[Command("unit")]
		[Description("Gets a unit prefivew and a link to the unitDB page.")]
		[RequireBotPermissions(Permissions.EmbedLinks)]
		public async Task UnitCommandAsync(CommandContext ctx,
			[Description("The ID of the unit to lookup.")]
			string unitId)
		{
			await RespondBasicError("Not implemented.");
		}
	}
}