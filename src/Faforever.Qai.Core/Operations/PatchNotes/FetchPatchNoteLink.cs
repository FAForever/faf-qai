using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Faforever.Qai.Core.Operations.PatchNotes
{
    public interface IFetchPatchNotesLinkOperation
    {
        Task<PatchNoteLink?> GetPatchNotesLinkAsync(string? version = null);
    }

    public class FetchPatchNotesLinkOperation : IFetchPatchNotesLinkOperation, IAutocompleteProvider
    {
        private const string CacheKey = "PatchNotesLinks";
        private const string PatchNotesUrl = "http://patchnotes.faforever.com";
        private IMemoryCache _cache;

        public FetchPatchNotesLinkOperation(IMemoryCache cache)
        {
            _cache = cache;
        }

        public async Task<PatchNoteLink?> GetPatchNotesLinkAsync(string? version = null)
        {
            var links = await GetLinks();

            if (!links.Any())
                return null;

            var link = !string.IsNullOrEmpty(version) ? links.FirstOrDefault(x => x.Version == version) : links.FirstOrDefault();

            return link;
        }

        private async Task<List<PatchNoteLink>> GetLinks()
        {
            var links = await _cache.GetOrCreateAsync(CacheKey, async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromHours(1);

                using var httpClient = new HttpClient();
                var body = await httpClient.GetStringAsync(PatchNotesUrl);

                return ExtractPatchNoteLinks(body);
            });

            return links ?? new List<PatchNoteLink>();
        }

        private static List<PatchNoteLink> ExtractPatchNoteLinks(string html)
        {
            var links = new List<PatchNoteLink>();

            // Regex pattern for extracting all href values
            string hrefPattern = @"href=""([^""]*)""";

            MatchCollection matches = Regex.Matches(html, hrefPattern);

            foreach (Match match in matches)
            {
                var hrefValue = match.Groups[1].Value;

                // Assuming that all patch note links are html files and starts with a number
                if (!string.IsNullOrEmpty(hrefValue) && char.IsDigit(hrefValue[0]) && hrefValue.EndsWith(".html"))
                {
                    var version = hrefValue.Split('.')[0]; // Extract version from the link
                    if (int.TryParse(version, out var versionInt))
                        links.Add(new PatchNoteLink { Version = version, Url = PatchNotesUrl + '/' + hrefValue });
                }
            }

            return links;
        }


        public async Task<IEnumerable<DiscordAutoCompleteChoice>> Provider(AutocompleteContext ctx)
        {
           var choices = new List<DiscordAutoCompleteChoice>();
            var links = await GetLinks();
            var optionValue = ctx.OptionValue?.ToString();

            foreach (var link in links.Where(l => string.IsNullOrEmpty(optionValue) || l.Version.ToString().Contains(optionValue)))
                choices.Add(new DiscordAutoCompleteChoice(link.Version.ToString(), link.Version));

            return choices;
        }
    }

    public class PatchNoteLink
    {
        public string Version { get; set; } = default!;
        public string Url { get; set; } = default!;
    }
}
