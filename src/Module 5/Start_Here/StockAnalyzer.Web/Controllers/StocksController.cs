using System.Threading.Tasks;
using System.Web.Http;
using StockAnalyzer.Core;

namespace StockAnalyzer.Web.Controllers
{
    public class StocksController : ApiController
    {
        [Route("api/stocks/{ticker}")]
        public async Task<IHttpActionResult> Get(string ticker)
        {
            var store = new DataStore();
            
            var data = await store.LoadStocks().ConfigureAwait(false);

            if (!data.ContainsKey(ticker)) return NotFound();

            return Json(data[ticker]);
        }
    }
}