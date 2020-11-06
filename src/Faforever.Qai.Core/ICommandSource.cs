using System.Threading.Tasks;

namespace Faforever.Qai.Core {
	public interface ICommandSource {
		string Name { get; }
		Task Respond(string message);
	}
}