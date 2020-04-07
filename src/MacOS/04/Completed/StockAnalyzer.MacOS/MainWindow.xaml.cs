using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Newtonsoft.Json;
using StockAnalyzer.Core.Domain;
using StockAnalyzer.MacOS.Services;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace StockAnalyzer.MacOS
{
    public partial class MainWindow
    {
        #region Asynchronous Streams
        CancellationTokenSource cancellationTokenSource = null;

        private async void Search_Click(object sender, RoutedEventArgs e)
        {
            #region Before loading stock data
            var watch = new Stopwatch();
            watch.Start();
            StockProgress.IsVisible = true;
            StockProgress.IsIndeterminate = true;
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
                var tickers = Ticker.Text.Split(' ');

                var prices = new ObservableCollection<StockPrice>();

                Stocks.Items = prices;

                var service = new StockDiskStreamService();

                await foreach (var price in service.GetAllStockPrices(cancellationTokenSource.Token))
                {
                    if (tickers.Contains(price.Ticker))
                    {
                        prices.Add(price);
                    }
                }
            }
            catch (Exception ex)
            {
                Notes.Text += ex.Message + Environment.NewLine;
            }

            #region After stock data is loaded
            StocksStatus.Text = $"Loaded stocks for {Ticker.Text} in {watch.ElapsedMilliseconds}ms";
            StockProgress.IsVisible = false;
            Search.Content = "Search";
            #endregion
        }

        #endregion

        #region Processing a collection of data in parallel
        //CancellationTokenSource cancellationTokenSource = null;

        //private async void Search_Click(object sender, RoutedEventArgs e)
        //{
        //    #region Code to make sure Web API is running
        //    // This code is just here to make sure that you have started the web api as well!
        //    using (var client = new HttpClient())
        //    {
        //        try
        //        {
        //            var response = await client.GetAsync("http://localhost:61363");
        //        }
        //        catch (Exception)
        //        {
        //            var alert = MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow("StockAnalyzer.Web IS NOT RUNNING", "Ensure that StockAnalyzer.Web is running, expecting to be running on http://localhost:61363. You can configure the solution to start two projects by right clicking the StockAnalyzer solution in Visual Studio, select properties and then Mutliuple Startup Projects.");

        //            await alert.Show();
        //        }
        //    }
        //    #endregion

        //    #region Before loading stock data
        //    var watch = new Stopwatch();
        //    watch.Start();
        //    StockProgress.IsVisible = true;
        //    StockProgress.IsIndeterminate = true;

        //    Search.Content = "Cancel";
        //    #endregion

        //    #region Cancellation
        //    if (cancellationTokenSource != null)
        //    {
        //        cancellationTokenSource.Cancel();
        //        cancellationTokenSource = null;
        //        return;
        //    }

        //    cancellationTokenSource = new CancellationTokenSource();

        //    cancellationTokenSource.Token.Register(() =>
        //    {
        //        Notes.Text += "Cancellation requested" + Environment.NewLine;
        //    });
        //    #endregion

        //    try
        //    {
        //        #region Load One or Many Tickers
        //        var tickers = Ticker.Text.Split(',', ' ');

        //        var service = new StockService();

        //        var tickerLoadingTasks = new List<Task<IEnumerable<StockPrice>>>();
        //        foreach (var ticker in tickers)
        //        {
        //            var loadTask = service.GetStockPricesFor(ticker, cancellationTokenSource.Token);

        //            tickerLoadingTasks.Add(loadTask);
        //        }
        //        #endregion

        //        var loadedStocks = await Task.WhenAll(tickerLoadingTasks);

        //        var values = new ConcurrentBag<StockCalculation>();

        //        var executionResult = Parallel.ForEach(loadedStocks,
        //            new ParallelOptions { MaxDegreeOfParallelism = 2 },
        //            (stocks, state) =>
        //            {
        //                var ticker = stocks.First().Ticker;

        //                Debug.WriteLine($"Start processing {ticker}");

        //                if (ticker == "MSFT")
        //                {
        //                    Debug.WriteLine($"Found {ticker}, breaking");

        //                    state.Stop();

        //                    return;
        //                }

        //                if (state.IsStopped) return;

        //                var result = CalculateExpensiveComputation(stocks);

        //                var data = new StockCalculation
        //                {
        //                    Ticker = ticker,
        //                    Result = result
        //                };

        //                values.Add(data);

        //                Debug.WriteLine($"Completed processing {ticker}");
        //            });

        //        Notes.Text = $"Ran to complation: {executionResult.IsCompleted}" + Environment.NewLine;
        //        Notes.Text += $"Lowest break iteration: {executionResult.LowestBreakIteration}";

        //        Stocks.Items = values.ToArray();
        //    }
        //    catch (Exception ex)
        //    {
        //        Notes.Text += ex.Message + Environment.NewLine;
        //    }
        //    finally
        //    {
        //        cancellationTokenSource = null;
        //    }

        //    #region After stock data is loaded
        //    StocksStatus.Text = $"Loaded stocks for {Ticker.Text} in {watch.ElapsedMilliseconds}ms";
        //    StockProgress.IsVisible = false;
        //    Search.Content = "Search";
        //    #endregion
        //}
        #endregion

        #region Working with Shared Variables
        //static object syncRoot = new object();

        //CancellationTokenSource cancellationTokenSource = null;

        //private async void Search_Click(object sender, RoutedEventArgs e)
        //{
        //    #region Code to make sure Web API is running
        //    // This code is just here to make sure that you have started the web api as well!
        //    using (var client = new HttpClient())
        //    {
        //        try
        //        {
        //            var response = await client.GetAsync("http://localhost:61363");
        //        }
        //        catch (Exception)
        //        {
        //            var alert = MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow("StockAnalyzer.Web IS NOT RUNNING", "Ensure that StockAnalyzer.Web is running, expecting to be running on http://localhost:61363. You can configure the solution to start two projects by right clicking the StockAnalyzer solution in Visual Studio, select properties and then Mutliuple Startup Projects.");

        //            await alert.Show();
        //        }
        //    }
        //    #endregion

        //    #region Before loading stock data
        //    var watch = new Stopwatch();
        //    watch.Start();
        //    StockProgress.IsVisible = true;
        //    StockProgress.IsIndeterminate = true;

        //    Search.Content = "Cancel";
        //    #endregion

        //    #region Cancellation
        //    if (cancellationTokenSource != null)
        //    {
        //        cancellationTokenSource.Cancel();
        //        cancellationTokenSource = null;
        //        return;
        //    }

        //    cancellationTokenSource = new CancellationTokenSource();

        //    cancellationTokenSource.Token.Register(() =>
        //    {
        //        Notes.Text += "Cancellation requested" + Environment.NewLine;
        //    });
        //    #endregion

        //    try
        //    {
        //        #region Load One or Many Tickers
        //        var tickers = Ticker.Text.Split(',', ' ');

        //        var service = new StockService();

        //        var tickerLoadingTasks = new List<Task<IEnumerable<StockPrice>>>();
        //        foreach (var ticker in tickers)
        //        {
        //            var loadTask = service.GetStockPricesFor(ticker, cancellationTokenSource.Token);

        //            tickerLoadingTasks.Add(loadTask);
        //        }
        //        #endregion

        //        var loadedStocks = (await Task.WhenAll(tickerLoadingTasks));

        //        decimal total = 0;

        //        Parallel.ForEach(loadedStocks, stocks =>
        //        {
        //            var value = 0m;
        //            foreach (var stock in stocks)
        //            {
        //                value += Compute(stock);
        //            }

        //            lock (syncRoot)
        //            {
        //                total += value;
        //            }
        //        });

        //        Notes.Text = total.ToString();
        //    }
        //    catch (Exception ex)
        //    {
        //        Notes.Text += ex.Message + Environment.NewLine;
        //    }
        //    finally
        //    {
        //        cancellationTokenSource = null;
        //    }

        //    #region After stock data is loaded
        //    StocksStatus.Text = $"Loaded stocks for {Ticker.Text} in {watch.ElapsedMilliseconds}ms";
        //    StockProgress.IsVisible = false;
        //    Search.Content = "Search";
        //    #endregion
        //}
        #endregion

        #region Invoke operations in parallel
        //CancellationTokenSource cancellationTokenSource = null;

        //private async void Search_Click(object sender, RoutedEventArgs e)
        //{
        //    #region Code to make sure Web API is running
        //    // This code is just here to make sure that you have started the web api as well!
        //    using (var client = new HttpClient())
        //    {
        //        try
        //        {
        //            var response = await client.GetAsync("http://localhost:61363");
        //        }
        //        catch (Exception)
        //        {
        //            var alert = MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow("StockAnalyzer.Web IS NOT RUNNING", "Ensure that StockAnalyzer.Web is running, expecting to be running on http://localhost:61363. You can configure the solution to start two projects by right clicking the StockAnalyzer solution in Visual Studio, select properties and then Mutliuple Startup Projects.");

        //            await alert.Show();
        //        }
        //    }
        //    #endregion

        //    #region Before loading stock data
        //    var watch = new Stopwatch();
        //    watch.Start();
        //    StockProgress.IsVisible = true;
        //    StockProgress.IsIndeterminate = true;

        //    Search.Content = "Cancel";
        //    #endregion

        //    #region Cancellation
        //    if (cancellationTokenSource != null)
        //    {
        //        cancellationTokenSource.Cancel();
        //        cancellationTokenSource = null;
        //        return;
        //    }

        //    cancellationTokenSource = new CancellationTokenSource();

        //    cancellationTokenSource.Token.Register(() =>
        //    {
        //        Notes.Text += "Cancellation requested" + Environment.NewLine;
        //    });
        //    #endregion

        //    try
        //    {
        //        #region Load One or Many Tickers
        //        var tickers = Ticker.Text.Split(',', ' ');

        //        var service = new StockService();

        //        var tickerLoadingTasks = new List<Task<IEnumerable<StockPrice>>>();
        //        foreach (var ticker in tickers)
        //        {
        //            var loadTask = service.GetStockPricesFor(ticker, cancellationTokenSource.Token);

        //            tickerLoadingTasks.Add(loadTask);
        //        }
        //        #endregion

        //        var loadedStocks = (await Task.WhenAll(tickerLoadingTasks)).SelectMany(stock => stock);

        //        Parallel.Invoke(new ParallelOptions { CancellationToken = cancellationTokenSource.Token },
        //                            () =>
        //                            {
        //                                #region Starting
        //                                Debug.WriteLine("Starting Operation 1");
        //                                #endregion

        //                                CalculateExpensiveComputation(loadedStocks);

        //                                #region Ending
        //                                Debug.WriteLine("Completed Operation 1");
        //                                #endregion
        //                            },
        //                            () =>
        //                            {
        //                                #region Starting
        //                                Debug.WriteLine("Starting Operation 2");
        //                                #endregion

        //                                CalculateExpensiveComputation(loadedStocks);

        //                                #region Ending
        //                                Debug.WriteLine("Completed Operation 2");
        //                                #endregion
        //                            },

        //                            () =>
        //                            {
        //                                #region Starting
        //                                Debug.WriteLine("Starting Operation 3");
        //                                #endregion

        //                                CalculateExpensiveComputation(loadedStocks);

        //                                #region Ending
        //                                Debug.WriteLine("Completed Operation 3");
        //                                #endregion
        //                            },

        //                            () =>
        //                            {
        //                                #region Starting
        //                                Debug.WriteLine("Starting Operation 4");
        //                                #endregion

        //                                CalculateExpensiveComputation(loadedStocks);

        //                                #region Ending
        //                                Debug.WriteLine("Completed Operation 4");
        //                                #endregion
        //                            }
        //                        );

        //        Stocks.Items = loadedStocks;
        //    }
        //    catch (Exception ex)
        //    {
        //        Notes.Text += ex.Message + Environment.NewLine;
        //    }
        //    finally
        //    {
        //        cancellationTokenSource = null;
        //    }

        //    #region After stock data is loaded
        //    StocksStatus.Text = $"Loaded stocks for {Ticker.Text} in {watch.ElapsedMilliseconds}ms";
        //    StockProgress.IsVisible = false;
        //    Search.Content = "Search";
        //    #endregion
        //}
        #endregion

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

        #region Helpers
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

        private decimal Compute(StockPrice stock)
        {
            Thread.Yield();

            decimal x = 0;
            for (var a = 0; a < 10; a++)
            {
                for (var b = 0; b < 20; b++)
                {
                    x += a + stock.Change;
                }
            }

            return x;
        }
        #endregion

        private void Close_OnClick(object sender, RoutedEventArgs e)
        {
            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
            {
                desktopLifetime.Shutdown();
            }
        }

        public static void Open(string url)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                url = url.Replace("&", "^&");
                Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url);
            }
        }
    }

    public partial class MainWindow : Window
    {
        public DataGrid Stocks => this.FindControl<DataGrid>(nameof(Stocks));
        public ProgressBar StockProgress => this.FindControl<ProgressBar>(nameof(StockProgress));
        public TextBox Ticker => this.FindControl<TextBox>(nameof(Ticker));
        public Button Search => this.FindControl<Button>(nameof(Search));
        public TextBox Notes => this.FindControl<TextBox>(nameof(Notes));
        public TextBlock StocksStatus => this.FindControl<TextBlock>(nameof(StocksStatus));
        public TextBlock DataProvidedBy => this.FindControl<TextBlock>(nameof(DataProvidedBy));
        public TextBlock IEX => this.FindControl<TextBlock>(nameof(IEX));
        public TextBlock IEX_Terms => this.FindControl<TextBlock>(nameof(IEX_Terms));

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            IEX.PointerPressed += (e, a) => Open("https://iextrading.com/developer/");
            IEX_Terms.PointerPressed += (e, a) => Open("https://iextrading.com/api-exhibit-a/");
        }
    }

    class StockCalculation
    {
        public string Ticker { get; set; }
        public decimal Result { get; set; }
    }
}
