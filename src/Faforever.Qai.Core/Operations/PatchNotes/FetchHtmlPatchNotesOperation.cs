using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Faforever.Qai.Core.Operations.PatchNotes
{
    public class FetchHtmlPatchNotesOperation(IMemoryCache cache, HttpClient httpClient) : IFetchPatchNotesLinkOperation, IAutocompleteProvider
    {
        private const string CacheKey = "HtmlPatchNotesLinks";
        private const string PatchNotesUrl = "https://faforever.github.io/fa/changelog";
        private readonly IMemoryCache _cache = cache;
        private readonly HttpClient _httpClient = httpClient;

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
                var html = await _httpClient.GetStringAsync(PatchNotesUrl);
                return await ParsePatchNoteLinksFromHtml(html);
            }) ?? new List<PatchNoteLink>();
        }

        private static async Task<List<PatchNoteLink>> ParsePatchNoteLinksFromHtml(string html)
        {
            var config = Configuration.Default;
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(req => req.Content(html));

            var links = new List<PatchNoteLink>();

            var mainElement = document.QuerySelector("main");
            if (mainElement == null)
                return links;

            var patchNotesUl = mainElement.QuerySelector("h2#past-game-patches")?.NextElementSibling as IHtmlUnorderedListElement;
            if (patchNotesUl != null)
            {
                var patchLinks = ParsePatchLinksFromList(patchNotesUl);
                links.AddRange(patchLinks);
            }

            return links.OrderByDescending(l => l.Version).ToList();
        }

        private static bool IsYearHeader(IElement element)
        {
            if (string.IsNullOrEmpty(element.Id))
                return false;

            return element.Id.Length == 4 && int.TryParse(element.Id, out var year) && year >= 2000 && year <= 2100;
        }

        private static List<PatchNoteLink> ParsePatchLinksFromList(IHtmlUnorderedListElement? listElement)
        {
            var links = new List<PatchNoteLink>();
            var currentYear = DateTime.Now.Year;

            if (listElement == null)
                return links;

            foreach (var child in listElement.Children)
            {
                if (child.TagName == "H2" && IsYearHeader(child))
                {
                    if (int.TryParse(child.Id, out var year))
                    {
                        currentYear = year;
                    }
                    continue;
                }
                
                if (child.TagName == "LI")
                {
                    var listItem = child as IHtmlListItemElement;
                    var anchor = listItem?.QuerySelector("a.preview-title");
                    var dateSpan = listItem?.QuerySelector("span");

                    if (anchor != null && !string.IsNullOrEmpty(anchor.GetAttribute("href")))
                    {
                        var href = anchor.GetAttribute("href");
                        var version = ExtractVersionFromHref(href) ?? anchor.TextContent?.Trim();
                        var dateText = dateSpan?.TextContent?.Trim('(', ')') ?? "";

                        if (!string.IsNullOrEmpty(version))
                        {
                            var patchLink = new PatchNoteLink
                            {
                                Version = version,
                                Url = $"https://faforever.github.io{href}",
                                Date = ParseDate(dateText, currentYear)
                            };

                            links.Add(patchLink);
                        }
                    }
                }
            }

            return links;
        }

        private static string? ExtractVersionFromHref(string href)
        {
            var segments = href.Split('/');
            return segments.LastOrDefault();
        }

        private static DateTime ParseDate(string dateText, int year)
        {
            if (string.IsNullOrEmpty(dateText))
                return default;

            var formats = new[]
            {
                "MMM dd",
                "MMM d",
                "MMMM dd",
                "MMMM d"
            };

            foreach (var format in formats)
            {
                if (DateTime.TryParseExact($"{dateText} {year}", $"{format} yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                {
                    return date;
                }
            }

            if (DateTime.TryParse($"{dateText} {year}", out var fallbackDate))
            {
                return fallbackDate;
            }

            return default;
        }

        public async Task<IEnumerable<DiscordAutoCompleteChoice>> Provider(AutocompleteContext ctx)
        {
            var choices = new List<DiscordAutoCompleteChoice>();
            var links = await GetLinks();
            var optionValue = ctx.OptionValue?.ToString();

            foreach (var link in links.Where(l => string.IsNullOrEmpty(optionValue) || l.Version.Contains(optionValue)))
            {
                var displayText = link.Date != default
                    ? $"{link.Version} ({link.Date:yyyy-MM-dd})"
                    : link.Version;

                choices.Add(new DiscordAutoCompleteChoice(displayText, link.Version));
            }

            return choices;
        }
    }
}
