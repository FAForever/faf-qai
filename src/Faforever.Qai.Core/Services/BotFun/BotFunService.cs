
using Faforever.Qai.Core.Extensions;
using Faforever.Qai.Core.Structures.Configurations;

namespace Faforever.Qai.Core.Services.BotFun
{
    public class BotFunService(BotFunConfiguration config) : IBotFunService
    {
        public string GetRandomEightballResponse()
            => config.EightballPhrases.Random();

        public string GetRandomKickTaunt()
            => config.KickTaunts.Random();

        public string GetRandomSpamProtectionTaunt()
            => config.SpamProtectionTaunts.Random();

        public string GetRandomTaunt()
            => config.Taunts.Random();
    }
}
