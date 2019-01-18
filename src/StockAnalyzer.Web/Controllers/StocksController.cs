using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using StockAnalyzer.Core;
using StockAnalyzer.Core.Domain;

namespace StockAnalyzer.Web.Controllers
{
    public class StocksController : ApiController
    {
        [Route("api/stocks/{ticker}")]
        public async Task<IEnumerable<StockPrice>> Get(string ticker)
        {
            var store = new DataStore();

            var data = await store.LoadStocks();

            return data[ticker];
        }
    }
}
    