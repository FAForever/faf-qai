using System.Threading.Tasks;

namespace Faforever.Qai.Core {
	public interface ICommandParser {
		Task HandleMessage(ICommandSource source, string message);
	}
}