using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockAnalyzer.Core;

namespace StockAnalyzer.Web.Controllers
{
    public class StocksController : Controller
    {
        [Route("api/stocks/{ticker}")]
        [Produces("application/json")]
        public async Task<IActionResult> Get(string ticker)
        {
            var store = new DataStore("");

            var data = await store.LoadStocks();

            if (!data.ContainsKey(ticker)) return NotFound();

            return Json(data[ticker]);
        }
    }
}