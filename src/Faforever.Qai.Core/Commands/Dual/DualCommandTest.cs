using System.Threading.Tasks;

using Qmmands;

namespace Faforever.Qai.Core.Commands
{
	public class DualCommandTest : DualCommandModule
	{
		[Command("dualtest")]
		public async Task DualTestCommandAsync()
		{
			await Context.ReplyAsync("This is a test.");
		}
	}
}
