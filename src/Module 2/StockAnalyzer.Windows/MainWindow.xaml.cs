using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Navigation;
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
        

        private void Close_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        
        private async void Search_Click(object sender, RoutedEventArgs e)
        {
            var watch = new Stopwatch();
                
            watch.Start();
            // Step 0 - Blocking
            StockProgress.Visibility = Visibility.Visible;
            StockProgress.IsIndeterminate = true;

            Thread.Sleep(5000);

            var data = new[]
            {
                new StockPrice {Ticker = "MSFT", Volume = 1, Change = 1, ChangePercent = 100 }
            };

            StocksStatus.Text = $"Loaded stocks for {Ticker.Text} in {watch.ElapsedMilliseconds}ms";

            Stocks.ItemsSource = data;
            StockProgress.Visibility = Visibility.Hidden;

            // Step 1
            //var store = new DataStore();

            //var data = await store.LoadStocks();

            //Stocks.ItemsSource = data["MSFT"];

            // Step 2
            //StockProgress.Visibility = Visibility.Visible;
            //StockProgress.IsIndeterminate = true;
            //var service = new StockService();

            //var data = await service.GetStockPricesFor(Ticker.Text);

            //watch.Stop();

            //StocksStatus.Text = $"Loaded stocks for {Ticker.Text} in {watch.ElapsedMilliseconds}ms";

            //Stocks.ItemsSource = data;
            //StockProgress.Visibility = Visibility.Hidden;

            // Step 3 - Progress
            //IProgress<int> progress = new Progress<int>(x =>
            //{
            //    StockProgress.Visibility = Visibility.Visible;
            //    StockProgress.Minimum = 0;
            //    StockProgress.Maximum = 100;
            //    StockProgress.Value = x;
            //});

            //await Task.Run(async () =>
            //{
            //    for (var i = 0; i < 100; i++)
            //    {
            //        progress.Report(i);

            //        await Task.Delay(100);
            //    }
            //});

            //StockProgress.Visibility = Visibility.Hidden;
        }

        private void Hyperlink_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));

            e.Handled = true;
        }
    }
}
