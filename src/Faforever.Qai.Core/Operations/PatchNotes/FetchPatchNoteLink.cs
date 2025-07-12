using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.Caching.Memory;

namespace Faforever.Qai.Core.Operations.PatchNotes
{
    public class FetchPatchNotesLinkOperation : IFetchPatchNotesLinkOperation, IAutocompleteProvider
    {
        private const string CacheKey = "PatchNotesLinks";
        private const string PatchNotesJsonUrl = "https://patchnotes.faforever.com/assets/data/patches.json";
        private readonly IMemoryCache _cache;

        public FetchPatchNotesLinkOperation(IMemoryCache cache)
        {
            _cache = cache;
        }

        public async Task<PatchNoteLink?> GetPatchNotesLinkAsync(string? version = null)
        {
            var links = await GetLinks();
            if (!links.Any())
                return null;

            var link = !string.IsNullOrEmpty(version)
                ? links.FirstOrDefault(x => x.Version == version)
                : links.FirstOrDefault();

            return link;
        }

        private async Task<List<PatchNoteLink>> GetLinks()
        {
            return await _cache.GetOrCreateAsync(CacheKey, async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromHours(1);
                using var httpClient = new HttpClient();
                var json = await httpClient.GetStringAsync(PatchNotesJsonUrl);
                return ParsePatchNoteLinks(json);
            }) ?? new List<PatchNoteLink>();
        }

        private static List<PatchNoteLink> ParsePatchNoteLinks(string json)
        {
            var links = new List<PatchNoteLink>();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var patchData = JsonSerializer.Deserialize<PatchData>(json, options);

            if (patchData != null)
            {
                links.AddRange(ParsePatchCategory(patchData.Balance));
                links.AddRange(ParsePatchCategory(patchData.Game));
            }

            return [.. links.OrderByDescending(l => l.Version)];
        }

        private static IEnumerable<PatchNoteLink> ParsePatchCategory(List<PatchInfo>? patchInfos)
        {
            if (patchInfos == null) yield break;

            foreach (var patch in patchInfos)
            {
                yield return new PatchNoteLink
                {
                    Version = patch.Patch,
                    Url = $"https://patchnotes.faforever.com/{patch.Link}",
                    Date = DateTime.TryParse(patch.Date.TrimStart('-', ' '), out var date) ? date : default
                };
            }
        }

        public async Task<IEnumerable<DiscordAutoCompleteChoice>> Provider(AutocompleteContext ctx)
        {
            var choices = new List<DiscordAutoCompleteChoice>();
            var links = await GetLinks();
            var optionValue = ctx.OptionValue?.ToString();

            foreach (var link in links.Where(l => string.IsNullOrEmpty(optionValue) || l.Version.Contains(optionValue)))
            {
                choices.Add(new DiscordAutoCompleteChoice($"{link.Version} ({link.Date:yyyy-MM-dd})", link.Version));
            }

            return choices;
        }
    }

    public class PatchNoteLink
    {
        public string Version { get; set; } = default!;
        public string Url { get; set; } = default!;
        public DateTime Date { get; set; }
    }

    public class PatchData
    {
        public List<PatchInfo> Balance { get; set; } = [];
        public List<PatchInfo> Game { get; set; } = [];
    }

    public class PatchInfo
    {
        public string Patch { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
    }
}