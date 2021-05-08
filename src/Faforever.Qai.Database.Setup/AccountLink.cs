using System;
using System.Collections.Generic;

#nullable disable

namespace Faforever.Qai.Database.Setup
{
    public partial class AccountLink
    {
        public long Id { get; set; }
        public long FafId { get; set; }
        public string DiscordId { get; set; }
        public byte[] CreateTime { get; set; }
    }
}
