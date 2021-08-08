using Faforever.Qai.Core.Extensions;
using Faforever.Qai.Core.Operations.Clients;
using JsonApiSerializer;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Faforever.Qai.Core.Operations.FafApi
{
    public interface IFafApiClient
    {
        Task<IEnumerable<T>> GetAsync<T>(ApiQuery<T> query, bool useCache = false);
    }

    public class FafApiClient : IFafApiClient
    {
        readonly HttpClient client;

        const string cacheDir = @"fafcache";
        private const int CACHE_TIMEOUT = 60 * 60;

        public FafApiClient(ApiHttpClient client)
        {
            this.client = client.Client;
        }

        private static string GetCacheFile(string s)
        {
            var hash = MD5.HashData(Encoding.UTF8.GetBytes(s));

            return Path.Combine(cacheDir, BitConverter.ToString(hash).Replace("-", ""));
        }

        public async Task<IEnumerable<T>> GetAsync<T>(ApiQuery<T> query, bool useCache = false)
        {
            var uri = $"data/{query}";
            var cacheFile = GetCacheFile(uri);
            string json;

            DateTime lastWrite = File.Exists(cacheFile) ? File.GetLastWriteTime(cacheFile) : DateTime.MinValue;

            if (!useCache || (DateTime.Now - lastWrite).TotalSeconds > CACHE_TIMEOUT)
            {
                json = await client.GetStringAsync(uri);

                if (useCache)
                {
                    Directory.CreateDirectory(cacheDir);
                    await File.WriteAllTextAsync(cacheFile, json);
                }
            }
            else
            {
                json = await File.ReadAllTextAsync(cacheFile);
            }

            return JsonConvert.DeserializeObject<T[]>(json, new JsonApiSerializerSettings()) ?? Array.Empty<T>();
        }
    }

    public class ApiQuery<T>
    {
        static Dictionary<Type, string> endpoints = new()
        {
            { typeof(Game), "game" }
        };

        static Dictionary<Type, string> includes = new()
        {
            { typeof(Game), "playerStats,playerStats.player,playerStats.player.names,host,host.names,mapVersion,mapVersion.map,mapVersion.map.statistics,featuredMod" }
        };

        static Dictionary<WhereOp, string> opStrings = new()
        {
            { WhereOp.Equals, "==" },
            { WhereOp.GreaterThan, "=gt=" },
            { WhereOp.LessThan, "=lt=" }
        };

        Dictionary<string, string> args = new();

        public ApiQuery()
        {
        }

        public ApiQuery<T> Include(string s)
        {
            args["include"] = s;
            return this;
        }

        public ApiQuery<T> Where<TValue>(string fieldName, WhereOp op, TValue value)
        {
            var opStr = opStrings[op];
            var expr = $"{fieldName}{opStr}{FormatValue(value)}";

            if (args.ContainsKey("filter"))
                args["filter"] += $";{expr}";
            else
                args["filter"] = expr;

            return this;
        }

        public ApiQuery<T> Where<TValue>(string fieldName, TValue value) => Where(fieldName, WhereOp.Equals, value);

        private static string FormatValue<TValue>(TValue value)
        {
            if (value is DateTime dt)
                return dt.ToIso8601();

            return HttpUtility.UrlEncode(value?.ToString() ?? "");
        }

        public ApiQuery<T> Where<TValue>(Expression<Func<T, TValue>> expr, WhereOp opStr, TValue value)
        {
            if (expr.Body is not MemberExpression body)
                throw new ArgumentException($"Should be a member expression", nameof(expr));

            // mapVersion.id = 4871 seems to crash api
            Where(body.Member.Name.ToLower(), opStr, value);

            return this;
        }

        public ApiQuery<T> Limit(int n)
        {
            args["page[size]"] = n.ToString();
            return this;
        }

        public ApiQuery<T> Page(int n)
        {
            args["page[number]"] = n.ToString();
            return this;
        }

        public ApiQuery<T> Sort(string s)
        {
            args["sort"] = s;
            return this;
        }

        public override string ToString()
        {
            if (!endpoints.TryGetValue(typeof(T), out string? endpoint))
                endpoint = typeof(T).Name.ToLower();

            if (!args.ContainsKey("include") && includes.TryGetValue(typeof(T), out var inc))
                this.Include(inc);

            return QueryHelpers.AddQueryString(endpoint, args);
        }
    }

    public enum WhereOp
    {
        Equals,
        GreaterThan,
        LessThan,
    }
}
