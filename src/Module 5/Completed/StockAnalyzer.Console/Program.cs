using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace StockAnalyzer
{
    //  !! WARNING !!
    //
    //  THIS PROJECT REQUIRES YOU TO RUN .NET CORE 3.0
    //  
    //
    //  USING "async Task Main" IS A C# 7.1 FEATURE AND REQUIRES YOU TO CHANGE THE PROJECT VERSION TO C# 7.1.
    //  USING "await foreach" IS A C# 8.0 FEATURE AND REQUIRES YOU TO CHANGE THE PROJECT VERSION TO C# 8.0.
    //
    //  EDIT StockAnalyzer.Console.csproj AND CHANGE <LangVersion></LangVersion> TO "preview" LIKE SO: <LangVersion>preview</LangVersion>
    //
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.Write("Search for ticker: ");
            var ticker = Console.ReadLine();

            var store = new DataStore();
            await foreach(var stock in store.LoadStocks(ticker))
            {
                System.Console.WriteLine($"{stock.Ticker} {stock.Change}");
            }
        }
    }

    class DataStore
    {
        public async IAsyncEnumerable<StockPrice> LoadStocks(string ticker)
        {
            using var stream = new StreamReader(@"StockPrices_Small.csv");

            // Skip first line
            await stream.ReadLineAsync();

            string line;
            while((line = await stream.ReadLineAsync()) != null)
            {
                 var segments = line.Split(',');

                for (var i = 0; i < segments.Length; i++) segments[i] = segments[i].Trim('\'', '"');
                
                var price = new StockPrice
                {
                    Ticker = segments[0],
                    TradeDate = DateTime.ParseExact(segments[1], "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture),
                    Volume = Convert.ToInt32(segments[6]),
                    Change = Convert.ToDecimal(segments[7]),
                    ChangePercent = Convert.ToDecimal(segments[8]),
                };

                if(price.Ticker == ticker)
                {
                    yield return price;

                    await Task.Delay(200);
                }
            }
        }
    }

    public class StockPrice
    {
        public string Ticker  { get; set; }
        public DateTime TradeDate  { get; set; }
        public decimal? Open  { get; set; }
        public decimal? High  { get; set; }
        public decimal? Low  { get; set; }
        public decimal? Close  { get; set; }
        public int Volume  { get; set; }
        public decimal Change  { get; set; }
        public decimal ChangePercent { get; set; }
    }
}
