﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
                var tickers = Ticker.Text.Split(',', ' ');

                var service = new StockService();

                var tickerLoadingTasks = new List<Task<IEnumerable<StockPrice>>>();
                foreach (var ticker in tickers)
                {
                    var loadTask = service.GetStockPricesFor(ticker, cancellationTokenSource.Token);

                    tickerLoadingTasks.Add(loadTask);
                }
                var timeoutTask = Task.Delay(2000);

                var allStocksLoadingTask = Task.WhenAll(tickerLoadingTasks);

                var completedTask = await Task.WhenAny(timeoutTask, allStocksLoadingTask);
                
                Stocks.ItemsSource = allStocksLoadingTask.Result.SelectMany(stocks => stocks);
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
        
        #region Process a task as they complete
        //private async void Search_Click(object sender, RoutedEventArgs e)
        //{
        //    #region Before loading stock data
        //    var watch = new Stopwatch();
        //    watch.Start();
        //    StockProgress.Visibility = Visibility.Visible;
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
        //        var tickers = Ticker.Text.Split(',', ' ');

        //        var service = new StockService();
        //        var stocks = new ConcurrentBag<StockPrice>();

        //        var tickerLoadingTasks = new List<Task<IEnumerable<StockPrice>>>();
        //        foreach (var ticker in tickers)
        //        {
        //            var loadTask = service.GetStockPricesFor(ticker, cancellationTokenSource.Token)
        //                .ContinueWith(t => {
        //                    foreach (var stock in t.Result.Take(5)) stocks.Add(stock);

        //                    Dispatcher.Invoke(() => {
        //                        Stocks.ItemsSource = stocks.ToArray();
        //                    });

        //                    return t.Result;
        //                });

        //            tickerLoadingTasks.Add(loadTask);
        //        }

        //        await Task.WhenAll(tickerLoadingTasks);

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
        //    StockProgress.Visibility = Visibility.Hidden;
        //    Search.Content = "Search";
        //    #endregion
        //}
        #endregion

        #region Knowing when All or Any Task completes
        //private async void Search_Click(object sender, RoutedEventArgs e)
        //{
        //    #region Before loading stock data
        //    var watch = new Stopwatch();
        //    watch.Start();
        //    StockProgress.Visibility = Visibility.Visible;
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

        //    cancellationTokenSource.Token.Register(() => {
        //        Notes.Text += "Cancellation requested" + Environment.NewLine;
        //    });
        //    #endregion

        //    try
        //    {
        //        var tickers = Ticker.Text.Split(',', ' ');

        //        var service = new StockService();

        //        var tickerLoadingTasks = new List<Task<IEnumerable<StockPrice>>>();
        //        foreach (var ticker in tickers)
        //        {
        //            var loadTask = service.GetStockPricesFor(ticker, cancellationTokenSource.Token);

        //            tickerLoadingTasks.Add(loadTask);
        //        }
        //        var timeoutTask = Task.Delay(2000);

        //        var allStocksLoadingTask = Task.WhenAll(tickerLoadingTasks);

        //        var completedTask = await Task.WhenAny(timeoutTask, allStocksLoadingTask);

        //        if(completedTask == timeoutTask)
        //        {
        //            cancellationTokenSource.Cancel();
        //            cancellationTokenSource = null;
        //            throw new Exception("Timeout!");
        //        }

        //        Stocks.ItemsSource = allStocksLoadingTask.Result.SelectMany(stocks => stocks);
        //    }
        //    catch(Exception ex)
        //    {
        //        Notes.Text += ex.Message + Environment.NewLine;
        //    }
        //    finally
        //    {
        //        cancellationTokenSource = null;
        //    }

        //    #region After stock data is loaded
        //    StocksStatus.Text = $"Loaded stocks for {Ticker.Text} in {watch.ElapsedMilliseconds}ms";
        //    StockProgress.Visibility = Visibility.Hidden;
        //    Search.Content = "Search";
        //    #endregion
        //}
        #endregion

        #region Handeling Success or Failure
        //private void Search_Click(object sender, RoutedEventArgs e)
        //{
        //    #region Before loading stock data
        //    var watch = new Stopwatch();
        //    watch.Start();
        //    StockProgress.Visibility = Visibility.Visible;
        //    StockProgress.IsIndeterminate = true;

        //    Search.Content = "Cancel";
        //    #endregion

        //    if(cancellationTokenSource != null)
        //    {
        //        cancellationTokenSource.Cancel();
        //        cancellationTokenSource = null;
        //        return;
        //    }

        //    cancellationTokenSource = new CancellationTokenSource();

        //    cancellationTokenSource.Token.Register(() => {
        //        Notes.Text = "Cancellation requested";
        //    });

        //    var loadLinesTask = SearchForStocks(cancellationTokenSource.Token);

        //    var processStocksTask = loadLinesTask.ContinueWith(t => {

        //        var lines = t.Result;

        //        var data = new List<StockPrice>();

        //        foreach (var line in lines.Skip(1))
        //        {
        //            var segments = line.Split(',');

        //            for (var i = 0; i < segments.Length; i++) segments[i] = segments[i].Trim('\'', '"');

        //            var price = new StockPrice
        //            {
        //                Ticker = segments[0],
        //                TradeDate = Convert.ToDateTime(segments[1]),
        //                Volume = Convert.ToInt32(segments[6]),
        //                Change = Convert.ToDecimal(segments[7]),
        //                ChangePercent = Convert.ToDecimal(segments[8]),
        //            };

        //            data.Add(price);
        //        }

        //        Dispatcher.Invoke(() =>
        //        {
        //            Stocks.ItemsSource = data.Where(price => price.Ticker == Ticker.Text);
        //        });
        //    }, 
        //        cancellationTokenSource.Token,
        //        TaskContinuationOptions.OnlyOnRanToCompletion,
        //        TaskScheduler.Current);

        //    loadLinesTask.ContinueWith(t => 
        //    {
        //        Dispatcher.Invoke(() => {
        //            Notes.Text = t.Exception.InnerException.Message;
        //        });
        //    }, TaskContinuationOptions.OnlyOnFaulted);

        //    processStocksTask.ContinueWith(_ => {

        //        Dispatcher.Invoke(() =>
        //        {
        //            #region After stock data is loaded
        //            StocksStatus.Text = $"Loaded stocks for {Ticker.Text} in {watch.ElapsedMilliseconds}ms, loaded {loadLinesTask.Result.Count} rows";
        //            StockProgress.Visibility = Visibility.Hidden;
        //            Search.Content = "Search";
        //            #endregion
        //        });
        //    });
        //}
        #endregion


        private Task<List<string>> SearchForStocks(CancellationToken cancellationToken)
        {
            var loadLinesTask = Task.Run(async () => 
            {
                var lines = new List<string>();

                using (var stream = new StreamReader(File.OpenRead(@"C:\Code\Pluralsight\StockData\StockPrices_small.csv")))
                {
                    string line;
                    while ((line = await stream.ReadLineAsync()) != null)
                    {
                        if(cancellationToken.IsCancellationRequested)
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