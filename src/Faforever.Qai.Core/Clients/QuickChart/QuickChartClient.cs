using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;

namespace Faforever.Qai.Core.Clients.QuickChart;

public class QuickChartClient
{
    public async Task<byte[]> GetChartAsync(QuickChartRequest request)
    {
        using var httpClient = new System.Net.Http.HttpClient();
        var json = JsonConvert.SerializeObject(request, jsonSettings);
        var content = new System.Net.Http.StringContent(json, Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync("https://quickchart.io/chart", content);
        return await response.Content.ReadAsByteArrayAsync();
    }

    private readonly JsonSerializerSettings jsonSettings = new JsonSerializerSettings()
    {
        NullValueHandling = NullValueHandling.Ignore,
        DefaultValueHandling = DefaultValueHandling.Include,
        ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
    };
}
