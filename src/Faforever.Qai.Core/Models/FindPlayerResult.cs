using System.Collections.Generic;

namespace Faforever.Qai.Core.Models
{

    public class FindPlayerResult
    {
        public FindPlayerResult()
        {
            Usernames = new List<string>();
        }

        public List<string> Usernames { get; set; }
    }
}