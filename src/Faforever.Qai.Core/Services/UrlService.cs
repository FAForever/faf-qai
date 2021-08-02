using Faforever.Qai.Core.Structures.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Faforever.Qai.Core.Services
{
	public interface IUrlService
	{
		public UrlEntry? FindUrl(string search);
	}

	public class UrlConfiguration
	{
		public Dictionary<string, UrlEntry> Urls { get; set; } = new();
		public Dictionary<string, string> Synonymes { get; set; } = new();
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
			if (_urlConfig.Urls.TryGetValue(search, out var entry))
				return entry;

			var first = _urlConfig.Urls.Select(kv => new { Entry = kv.Value, Distance = Levenshtein.Distance(kv.Key, search) }).OrderBy(lv => lv.Distance).FirstOrDefault(lv => lv.Distance < 5);

			return first?.Entry;
		}
	}
}
