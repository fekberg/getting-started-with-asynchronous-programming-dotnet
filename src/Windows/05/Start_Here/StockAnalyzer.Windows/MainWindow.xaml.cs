using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using Newtonsoft.Json;
using StockAnalyzer.Core.Domain;
using StockAnalyzer.Windows.Services;

namespace StockAnalyzer.Windows
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        CancellationTokenSource cancellationTokenSource = null;

        private async void Search_Click(object sender, RoutedEventArgs e)
        {
            #region Before loading stock data
            var watch = new Stopwatch();
            watch.Start();
            StockProgress.Visibility = Visibility.Visible;

            Search.Content = "Cancel";
            #endregion

            #region Cancellation
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource = null;
                return;
            }

            cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Token.Register(() =>
            {
                Notes.Text += "Cancellation requested" + Environment.NewLine;
            });
            #endregion

            try
            {
                StockProgress.IsIndeterminate = false;
                StockProgress.Value = 0;
                StockProgress.Maximum = Ticker.Text.Split(',', ' ').Count();

                await LoadStocks();
            }
            catch (Exception ex)
            {
                Notes.Text += ex.Message + Environment.NewLine;
            }
            finally
            {
                cancellationTokenSource = null;
            }

            #region After stock data is loaded
            StocksStatus.Text = $"Loaded stocks for {Ticker.Text} in {watch.ElapsedMilliseconds}ms";
            StockProgress.Visibility = Visibility.Hidden;
            Search.Content = "Search";
            #endregion
        }

        private async Task LoadStocks()
        {
            var tickers = Ticker.Text.Split(',', ' ');

            var service = new StockService();

            var tickerLoadingTasks = new List<Task<IEnumerable<StockPrice>>>();
            
            foreach (var ticker in tickers)
            {
                var loadTask = service.GetStockPricesFor(ticker, cancellationTokenSource.Token);

                tickerLoadingTasks.Add(loadTask);
            }

            var allStocks = await Task.WhenAll(tickerLoadingTasks);

            Stocks.ItemsSource = allStocks.SelectMany(stocks => stocks);
        }
        
        private Task<List<string>> SearchForStocks(CancellationToken cancellationToken)
        {
            var loadLinesTask = Task.Run(async () =>
            {
                var lines = new List<string>();

                using (var stream = new StreamReader(File.OpenRead(@"StockPrices_small.csv")))
                {
                    string line;
                    while ((line = await stream.ReadLineAsync()) != null)
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            return lines;
                        }
                        lines.Add(line);
                    }
                }

                return lines;
            }, cancellationToken);

            return loadLinesTask;
        }

        public Task<IEnumerable<StockPrice>> GetStocksFor(string ticker)
        {        
            ThreadPool.QueueUserWorkItem(_ =>
            {
                try
                {
                    var prices = new List<StockPrice>();

                    var lines = File.ReadAllLines(@"StockPrices_Small.csv");

                    foreach (var line in lines.Skip(1))
                    {
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
                        prices.Add(price);
                    }

                }
                catch (Exception ex)
                {
                }
            });

            // TODO: Change this
            return Task.FromResult<IEnumerable<StockPrice>>(null); ;
        }

        Random random = new Random();
        private decimal CalculateExpensiveComputation(IEnumerable<StockPrice> stocks)
        {
            Thread.Yield();

            var computedValue = 0m;

            foreach (var stock in stocks)
            {
                for (int i = 0; i < stocks.Count() - 2; i++)
                {
                    for (int a = 0; a < random.Next(50, 60); a++)
                    {
                        computedValue += stocks.ElementAt(i).Change + stocks.ElementAt(i + 1).Change;
                    }
                }
            }

            return computedValue;
        }
        
        private void Hyperlink_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));

            e.Handled = true;
        }

        private void Close_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}