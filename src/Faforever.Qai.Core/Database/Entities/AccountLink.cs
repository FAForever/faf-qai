using System.ComponentModel.DataAnnotations;

namespace Faforever.Qai.Core.Database.Entities
{
    public class AccountLink
    {
        [Key]
        public ulong DiscordId { get; set; }
        public int FafId { get; set; }
        public string? FafUsername { get; set; }
        public string? DiscordUsername { get; set; }
    }
}
