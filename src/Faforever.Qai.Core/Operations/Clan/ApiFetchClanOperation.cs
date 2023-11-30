using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Faforever.Qai.Core.Models;
using Faforever.Qai.Core.Operations.FafApi;
using Newtonsoft.Json.Linq;

namespace Faforever.Qai.Core.Operations.Clan
{
    public class ApiFetchClanOperation : IFetchClanOperation
    {
        private readonly FafApiClient _api;

        public ApiFetchClanOperation(FafApiClient api)
        {
            _api = api;
        }

        public async Task<FetchClanResult?> FetchClanAsync(string clanName)
        {
            var orFilter = new Dictionary<string, string>()
            {
                ["tag"] = clanName,
                ["name"] = clanName
            };

            var query = new ApiQuery<FafApi.Clan>()
                .WhereOr(orFilter)
                .Include("founder,leader,memberships.player,memberships.clan,memberships.player");

            var s = query.ToString();
            var clans = await _api.GetAsync(query);
            var clan = clans?.FirstOrDefault();
            if (clan is null)
                return null;

            var result = new FetchClanResult()
            {
                Clan = new()
                {
                    CreatedDate = clan.CreateTime,
                    Description = clan.Description,
                    Name = clan.Name,
                    Tag = clan.Tag,
                    URL = clan.WebsiteUrl,
                    Id = clan.Id
                },
                Members = clan.Memberships.Select(m => new ShortPlayerData()
                {
                    JoinDate = m.CreateTime,
                    Username = m.Player.Login
                }).ToList()
            };

            return result;
        }
    }
}
