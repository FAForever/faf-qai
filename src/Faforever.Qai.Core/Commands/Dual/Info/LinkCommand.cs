using System.Threading.Tasks;

using DSharpPlus.Entities;

using Faforever.Qai.Core.Commands.Context;
using Faforever.Qai.Core.Models;
using Faforever.Qai.Core.Operations.Units;
using Faforever.Qai.Core.Services;
using Qmmands;

namespace Faforever.Qai.Core.Commands.Dual.Info
{
	public class LinkCommand : DualCommandModule
	{
		private readonly IUrlService _urlService;

		public LinkCommand(IUrlService urlService)
		{
			this._urlService = urlService;
		}

		[Command("link")]
		[Description("Search for a specifik link")]
		public async Task UrlCommandAsync([Remainder] string search)
		{
			var result = _urlService.FindUrl(search);

			if (result is not null)
				await Context.ReplyAsync($"{result.Title} {result.Url}");
			else
				await Context.ReplyAsync("Link not found");
		}
	}
}
