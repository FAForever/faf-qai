using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Faforever.Qai.Core.Models;
using Faforever.Qai.Core.Operations.Clients;

using Newtonsoft.Json.Linq;

namespace Faforever.Qai.Core.Operations.Clan
{
    public class ApiFetchClanOperation : IFetchClanOperation
    {
        private readonly ApiHttpClient _api;

        public ApiFetchClanOperation(ApiHttpClient api)
        {
            _api = api;
        }

        public async Task<FetchClanResult?> FetchClanAsync(string clan)
        {
            string data = await _api.Client
                .GetStringAsync($"https://api.faforever.com/data/clan?include=" +
                $"memberships.player&fields[player]=login&fields[clanMembership]=createTime,player&fields[clan]=" +
                $"name,description,websiteUrl,createTime,tag,leader&filter=tag=='{clan}'");

            var json = JObject.Parse(data);

            if (json["data"]?.Count() <= 0)
            {
                data = await _api.Client
                    .GetStringAsync($"https://api.faforever.com/data/clan?include=" +
                    $"memberships.player&fields[player]=login&fields[clanMembership]=createTime,player&fields[clan]=" +
                    $"name,description,websiteUrl,createTime,tag,leader&filter=name=='{clan}'");

                json = JObject.Parse(data);
            }

            JToken? clanRes = json["data"]?.First;

            if (clanRes is null) return null;

            JToken? attr = clanRes["attributes"];

            if (attr is null) return null;

            FetchClanResult res = new()
            {
                Clan = new()
                {
                    CreatedDate = attr["createTime"]?.ToObject<DateTime>(),
                    Description = attr["description"]?.ToString(),
                    Name = attr["name"]?.ToString(),
                    Tag = attr["tag"]?.ToString(),
                    URL = attr["websiteUrl"]?.ToString(),
                    Id = clanRes["id"]?.ToObject<long>()
                }
            };

            List<JToken>? memberships = json["included"]?.Where(x => x["type"]?.ToString() == "clanMembership")?.ToList();
            List<JToken>? players = json["included"]?.Where(x => x["type"]?.ToString() == "player")?.ToList();

            if (memberships is not null && players is not null)
            {
                foreach (var member in memberships)
                {
                    var p = players.FirstOrDefault(x => x["id"]?.ToObject<long>()
                        == member["relationships"]?["player"]?["data"]?["id"]?.ToObject<long>());

                    if (p is not null)
                    {
                        res.Members.Add(new()
                        {
                            JoinDate = member["attributes"]?["createTime"]?.ToObject<DateTime>(),
                            Username = p["attributes"]?["login"]?.ToString()
                        });
                    }
                }

                res.Clan.Size = res.Members.Count;
            }

            return res;
        }
    }
}
