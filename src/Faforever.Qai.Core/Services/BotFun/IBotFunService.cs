using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faforever.Qai.Core.Services.BotFun
{
	public interface IBotFunService
	{
		public string GetRandomTaunt();
		public string GetRandomSpamProtectionTaunt();
		public string GetRandomKickTaunt();
		public string GetRandomEightballResponse();
	}
}
