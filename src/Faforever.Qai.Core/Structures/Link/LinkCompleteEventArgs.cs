using Faforever.Qai.Core.Database.Entities;

namespace Faforever.Qai.Core.Structures.Link
{
    public class LinkCompleteEventArgs
    {
        public bool Complete { get; internal set; }
        public ulong Guild { get; internal set; }
        public AccountLink? Link { get; internal set; }
    }
}
