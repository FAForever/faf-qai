using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Faforever.Qai.Core.Extensions;
using Faforever.Qai.Core.Structures.Configurations;

namespace Faforever.Qai.Core.Services.BotFun
{
	public class BotFunService : IBotFunService
	{
		private readonly BotFunConfiguration _config;

		public BotFunService(BotFunConfiguration config)
		{
			_config = config;
		}

		public string GetRandomEightballResponse()
			=> _config.EightballPhrases.Random();

		public string GetRandomKickTaunt() 
			=> _config.KickTaunts.Random();

		public string GetRandomSpamProtectionTaunt() 
			=> _config.SpamProtectionTaunts.Random();

		public string GetRandomTaunt() 
			=> _config.Taunts.Random();
	}
}
