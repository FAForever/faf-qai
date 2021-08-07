using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Faforever.Qai.Core.Services
{
    public interface IUrlService
    {
        public UrlEntry? FindUrl(string search);
        public UrlEntry? FindWikiUrl(string search);
    }

    public class UrlConfiguration
    {
        [JsonProperty("urls")]
        public Dictionary<string, UrlEntry> Urls { get; set; } = new();
        [JsonProperty("wiki_urls")]
        public Dictionary<string, UrlEntry> WikiUrls { get; set; } = new();
    }

    public class UrlEntry
    {
        public string Title { get; set; } = default!;
        public string Url { get; set; } = default!;
    }

    public class UrlService : IUrlService
    {
        private UrlConfiguration _urlConfig;

        public UrlService(UrlConfiguration wikiConfig)
        {
            this._urlConfig = wikiConfig;
        }

        public UrlEntry? FindUrl(string search)
        {
            return FindUrlEntry(_urlConfig.Urls, search);
        }

        public UrlEntry? FindWikiUrl(string search)
        {
            return FindUrlEntry(_urlConfig.WikiUrls, search);
        }

        private static UrlEntry? FindUrlEntry(IDictionary<string, UrlEntry> urls, string search)
        {
            if (urls.TryGetValue(search, out var entry))
                return entry;

            var first = urls.Select(kv => new { Entry = kv.Value, Distance = Levenshtein.Distance(kv.Key, search) }).OrderBy(lv => lv.Distance).FirstOrDefault(lv => lv.Distance < 5);

            return first?.Entry;
        }
    }
}
