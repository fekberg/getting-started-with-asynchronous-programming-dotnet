using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Navigation;
using StockAnalyzer.Core.Domain;

namespace StockAnalyzer.Windows
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        
        private void Search_Click(object sender, RoutedEventArgs e)
        {
            var watch = new Stopwatch();

            watch.Start();

            Thread.Sleep(5000);

            var data = new[]
            {
                new StockPrice
                {
                    Ticker = "MSFT",
                    Volume = 1,
                    Change = 1,
                    ChangePercent = 100
                }
            };

            StocksStatus.Text = $"Loaded stocks for {Ticker.Text} in {watch.ElapsedMilliseconds}ms";

            Stocks.ItemsSource = data;
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
