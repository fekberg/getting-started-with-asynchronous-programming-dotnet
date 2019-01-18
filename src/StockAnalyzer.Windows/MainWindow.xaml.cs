using System.Diagnostics;
using System.Windows;
using StockAnalyzer.Core;
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

            // Step 1
            //var store = new DataStore();

            //var data = await store.LoadStocks();

            //Stocks.ItemsSource = data["MSFT"];

            // Step 2
            var service = new StockService();

            var data = await service.GetStockPricesFor(Ticker.Text);

            watch.Stop();

            StocksStatus.Text = $"Loaded stocks for {Ticker.Text} in {watch.ElapsedMilliseconds}ms";

            Stocks.ItemsSource = data;
        }
    }
}
