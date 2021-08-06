using System.Threading.Tasks;

using Qmmands;

namespace Faforever.Qai.Core.Commands.Dual.Fun
{
	public class RollDiceCommand : DualCommandModule
	{
		[Command("roll")]
		[Description("Rolls a dice (1d100)")]
		public async Task RollDiceCommandAsync()
		{
			var num = QCommandsHandler.Rand.Next(1, 101);
			await Context.ReplyAsync($"Rolled a dice: {num}");
		}
	}
}
