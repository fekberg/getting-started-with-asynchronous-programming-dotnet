using System.Threading.Tasks;
using System.Web.Mvc;
using StockAnalyzer.Core;

namespace StockAnalyzer.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }





        [Route("Stock/{ticker}")]
        public async Task<ActionResult> Stock(string ticker)
        {
            if (string.IsNullOrWhiteSpace(ticker)) ticker = "MSFT";

            ViewBag.Title = $"Stock Details for {ticker}";

            var store = new DataStore();

            var data = await store.LoadStocks();

            return View(data[ticker]);
        }








    }
}
