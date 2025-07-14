using System.Threading.Tasks;
using Faforever.Qai.Core.Services;
using Qmmands;

namespace Faforever.Qai.Core.Commands.Dual.Info
{
    public class UrlCommand : DualCommandModule
    {
        private readonly IUrlService _urlService;

        public UrlCommand(IUrlService urlService)
        {
            this._urlService = urlService;
        }

        [Command("url")]
        [Description("Search for a specific url")]
        public async Task UrlCommandAsync([Remainder] string search)
        {
            var result = _urlService.FindUrl(search);

            if (result is not null)
                await Context.ReplyAsync($"{result.Title} {result.Url}");
            else
                await Context.ReplyAsync("Url not found");
        }

        [Command("wiki")]
        [Description("Search for a specific wiki url")]
        public async Task WikiCommandAsync([Remainder] string search)
        {
            var result = _urlService.FindWikiUrl(search);

            if (result is not null)
                await Context.ReplyAsync($"{result.Title} {result.Url}");
            else
                await Context.ReplyAsync("Wiki link not found");
        }
    }
}
