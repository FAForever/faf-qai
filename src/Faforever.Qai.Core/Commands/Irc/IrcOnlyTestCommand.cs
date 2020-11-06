using System.Threading.Tasks;

using Qmmands;

namespace Faforever.Qai.Core.Commands
{
	public class IrcOnlyTestCommand : IrcCommandModule
	{
		[Command("irctest")]
		public async Task IrcTestCommand()
		{
			await Context.ReplyAsync("This is a test response.");
			Context.Client.SendMessage(Context.Name, "Another test response.");
		}
	}
}
