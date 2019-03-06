﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace StockAnalyzer.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var store = new DataStore();
            await foreach(var stock in store.LoadStocks())
            {
                System.Console.WriteLine($"{stock.Ticker} {stock.Change}");
            }
        }
    }

    class DataStore
    {
        public async IAsyncEnumerable<StockPrice> LoadStocks(string ticker)
        {
            using var stream = new StreamReader(@"C:\Code\Pluralsight\StockData\StockPrices_Small.csv");

            string line;
            while((line = await stream.ReadLineAsync()) != null)
            {
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

                if(price.Ticker == ticker)
                {
                    yield return price;
                }

                await Task.Delay(400);
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
