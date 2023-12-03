using System.IO;
using System.Threading.Tasks;
using Faforever.Qai.Core.Clients.QuickChart;
using NUnit.Framework;

namespace Faforever.Qai.Core.Tests.Chart
{
    public class CreateRatingChartTests
    {
        /*
        [TestCase]
        public async Task Test()
        {
            var client = new QuickChartClient();
            var req = ChartTemplate.CreateRatingChartRequest("Crotalus - Global rating");
            var jsonRatingData = File.ReadAllText(@"C:\temp\ratingdata.json");
            var jsonGameData = File.ReadAllText(@"C:\temp\gamedata.json");
            var ratingData = Newtonsoft.Json.JsonConvert.DeserializeObject<CandleStickDataPoint[]>(jsonRatingData);
            var gameData = Newtonsoft.Json.JsonConvert.DeserializeObject<DataPoint[]>(jsonGameData);

            req.Chart.Data.Datasets[0].Data = ratingData;
            req.Chart.Data.Datasets[1].Data = gameData;

            var chartBytes = await client.GetChartAsync(req);
            File.WriteAllBytes(@"C:\temp\chart.png", chartBytes);
        }
        */
    }
}
