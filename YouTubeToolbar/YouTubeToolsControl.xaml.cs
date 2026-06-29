namespace YouTubeToolbar
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;

    /// <summary>
    /// Interaction logic for YouTubeToolsControl. Hosts an embedded YouTube view.
    /// </summary>
    public partial class YouTubeToolsControl : UserControl
    {
        private const string HomeUrl = "https://www.youtube.com/";
        private const string SearchUrl = "https://www.youtube.com/results?search_query=";
        private const string SearchPlaceholder = "Search YouTube   ";

        /// <summary>
        /// Initializes a new instance of the <see cref="YouTubeToolsControl"/> class.
        /// </summary>
        public YouTubeToolsControl()
        {
            this.InitializeComponent();
            this.Loaded += this.OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Initialize the WebView2 environment and open YouTube.
                await this.webView.EnsureCoreWebView2Async(null);
                this.Navigate(HomeUrl);
            }
            catch
            {
                // WebView2 runtime missing or failed to start. The Browser button
                // still works as a fallback.
            }
        }

        /// <summary>
        /// Handles all the top-bar button clicks.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void buttonClickedControll(object sender, RoutedEventArgs e)
        {
            string content = (sender as Button).Name.ToString();

            switch (content)
            {
                case "btnSearch":
                    this.DoSearch();
                    break;

                case "btnHome":
                    this.Navigate(HomeUrl);
                    break;

                case "btnExternal":
                    this.OpenInExternalBrowser();
                    break;
            }
        }

        private void txbSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.DoSearch();
            }
        }

        private void DoSearch()
        {
            var query = this.txbSearch.Text;
            if (string.IsNullOrEmpty(query) || query == SearchPlaceholder)
            {
                return;
            }

            this.Navigate(SearchUrl + Uri.EscapeDataString(query.Trim()));
        }

        private void Navigate(string url)
        {
            try
            {
                if (this.webView.CoreWebView2 != null)
                {
                    this.webView.CoreWebView2.Navigate(url);
                }
                else
                {
                    this.webView.Source = new Uri(url);
                }
            }
            catch
            {
            }
        }

        private void OpenInExternalBrowser()
        {
            try
            {
                var url = HomeUrl;
                var query = this.txbSearch.Text;
                if (!string.IsNullOrEmpty(query) && query != SearchPlaceholder)
                {
                    url = SearchUrl + Uri.EscapeDataString(query.Trim());
                }

                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true,
                });
            }
            catch
            {
            }
        }

        private void txbSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            if (this.txbSearch.Text == SearchPlaceholder)
            {
                this.txbSearch.Text = string.Empty;
                this.txbSearch.FontStyle = FontStyles.Normal;
                this.txbSearch.Foreground = Brushes.Black;
            }
        }

        private void txbSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.txbSearch.Text))
            {
                this.txbSearch.Text = SearchPlaceholder;
                this.txbSearch.FontStyle = FontStyles.Italic;
                this.txbSearch.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF787878");
            }
        }
    }
}
