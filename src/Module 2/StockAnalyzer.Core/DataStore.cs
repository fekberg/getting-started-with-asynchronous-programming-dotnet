using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using StockAnalyzer.Core.Domain;

namespace StockAnalyzer.Core
{
    public class DataStore
    {
        public Dictionary<string, Company> Companies = new Dictionary<string, Company>();
        public static Dictionary<string, IEnumerable<StockPrice>> Stocks = new Dictionary<string, IEnumerable<StockPrice>>();

        public async Task<Dictionary<string, IEnumerable<StockPrice>>> LoadStocks()
        {
            if (Stocks.Any()) return Stocks;

            await LoadCompanies();

            var prices = await GetStockPrices();

            Stocks = prices
                .GroupBy(x => x.Ticker)
                .ToDictionary(x => x.Key, x => x.AsEnumerable());

            return Stocks;
        }

        private async Task LoadCompanies()
        {
            using (var stream = new StreamReader(File.OpenRead(@"C:\Code\Pluralsight\Module2\StockData\CompanyData.csv")))
            {
                // Skip first line in the CSV containing the Header
                await stream.ReadLineAsync();

                string line;
                while ((line = await stream.ReadLineAsync()) != null)
                {
                    #region Loading and Adding Company to In-Memory Dictionary

                    // Segments:
                    // Symbol,CompanyName,Exchange,Industry,Website,Description,CEO,IssueType,Sector

                    var segments = line.Split(',');

                    for (var i = 0; i < segments.Length; i++) segments[i] = segments[i].Trim('\'', '"');

                    var company = new Company
                    {
                        Symbol = segments[0],
                        CompanyName = segments[1],
                        Exchange = segments[2],
                        Industry = segments[3],
                        Website = segments[4],
                        Description = segments[5],
                        CEO = segments[6],
                        IssueType = segments[7],
                        Sector = segments[8]
                    };

                    if (!Companies.ContainsKey(segments[0]))
                    {
                        Companies.Add(segments[0], company);
                    }

                    #endregion
                }
            }
        }

        private static async Task<List<StockPrice>> GetStockPrices()
        {
            var prices = new List<StockPrice>();

            using (var stream = new StreamReader(File.OpenRead(@"C:\Code\Pluralsight\Module2\StockData\StockPrices_Small.csv")))
            {
                // Skip first line in the CSV
                await stream.ReadLineAsync(); 

                string line;
                while ((line = await stream.ReadLineAsync()) != null)
                {
                    #region Loading Stock Prices

                    // Segments:
                    //      Ticker,TradeDate,Open,High,Low,Close,Volume,Change,ChangePercent
                    var segments = line.Split(',');

                    for (var i = 0; i < segments.Length; i++) segments[i] = segments[i].Trim('\'', '"');
                    var price = new StockPrice
                    {
                        Ticker = segments[0],
                        TradeDate = Convert.ToDateTime(segments[1]),
                        Volume = Convert.ToInt32(segments[6]),
                        Change = Convert.ToDecimal(segments[7]),
                        ChangePercent = Convert.ToDecimal(segments[8]),
                    };
                    prices.Add(price);

                    #endregion
                }
            }

            return prices;
        }
    }
}
