using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StockAnalyzer.Core.Domain;

namespace StockAnalyzer.Windows.Services
{
    public class StockService
    {
        public async Task<IEnumerable<StockPrice>> GetStockPricesFor(string ticker)
        {
            using (var client = new HttpClient())
            {
                var result = await client.GetAsync($"http://localhost:61363/api/stocks/{ticker}");

                result.EnsureSuccessStatusCode();

                var content = await result.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<IEnumerable<StockPrice>>(content);
            }
        }
    }
}
