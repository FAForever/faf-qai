using System;

namespace Faforever.Qai.Core.Models
{
    public class LastSeenPlayerResult
    {
        public int Id { get; set; }
        public string Username { get; set; } = default!;
        public DateTime? SeenFaf { get; set; }
        public DateTime? SeenGame { get; set; }
    }
}