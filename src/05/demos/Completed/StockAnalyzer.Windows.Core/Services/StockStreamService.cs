using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using StockAnalyzer.Core.Domain;

namespace StockAnalyzer.Windows.Services
{
    public interface IStockStreamService
    {
        IAsyncEnumerable<StockPrice> 
            GetAllStockPrices(CancellationToken cancellationToken = default);
    }

    public class MockStockStreamService : IStockStreamService
    {
        public async IAsyncEnumerable<StockPrice> GetAllStockPrices(CancellationToken cancellationToken = default)
        {
            await Task.Delay(500);
            yield return new StockPrice { Ticker = "MSFT", Change = 0.5m, ChangePercent = 0.75m };

            await Task.Delay(500);
            yield return new StockPrice { Ticker = "MSFT", Change = 0.2m, ChangePercent = 0.15m };

            await Task.Delay(500);
            yield return new StockPrice { Ticker = "GOOG", Change = 0.3m, ChangePercent = 0.25m };

            await Task.Delay(500);
            yield return new StockPrice { Ticker = "GOOG", Change = 0.5m, ChangePercent = 0.65m };
        }
    }

    public class StockDiskStreamService : IStockStreamService
    {
        public async IAsyncEnumerable<StockPrice> GetAllStockPrices(CancellationToken cancellationToken = default)
        {
            using var stream = 
                new StreamReader(File.OpenRead(@"StockPrices_small.csv"));

            await stream.ReadLineAsync();

            string line;

            while ((line = await stream.ReadLineAsync()) != null)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                var segments = line.Split(',');
                for (var i = 0; i < segments.Length; i++) segments[i] = segments[i].Trim('\'', '"');

                var price = new StockPrice
                {
                    Ticker = segments[0],
                    TradeDate = DateTime.ParseExact(segments[1], "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture),
                    Volume = Convert.ToInt32(segments[6], CultureInfo.InvariantCulture),
                    Change = Convert.ToDecimal(segments[7], CultureInfo.InvariantCulture),
                    ChangePercent = Convert.ToDecimal(segments[8], CultureInfo.InvariantCulture),
                };

                yield return price;
            }
        }
    }

}
