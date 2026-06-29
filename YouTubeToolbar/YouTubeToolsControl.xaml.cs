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
        private const string GoogleUrl = "https://www.google.com/";
        private const string SearchUrl = "https://www.youtube.com/results?search_query=";
        private const string SearchPlaceholder = "Search YouTube   ";

        private bool isInitialized;

        // True when the user pressed Off. Blocks auto-resume so it stays
        // disconnected until the user explicitly turns it back on.
        private bool isOff;

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
            // Loaded fires every time the control re-attaches to the visual tree
            // (showing the tool window, switching tabs, redocking). Only navigate
            // on the first load so the page is not reloaded during the VS session.
            // The control is recreated on the next VS start, which reloads then.
            if (this.isInitialized)
            {
                return;
            }

            this.isInitialized = true;

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
                    this.TurnOn();
                    this.DoSearch();
                    break;

                case "btnHome":
                    this.TurnOn();
                    this.Navigate(HomeUrl);
                    break;

                case "btnGoogle":
                    this.TurnOn();
                    this.Navigate(GoogleUrl);
                    break;

                case "btnExternal":
                    this.OpenInExternalBrowser();
                    break;

                case "btnOff":
                    // Toggle: Off suspends/disconnects, On resumes.
                    if (this.isOff)
                    {
                        this.TurnOn();
                    }
                    else
                    {
                        this.TurnOff();
                    }

                    break;
            }
        }

        /// <summary>
        /// Disconnects the view: shows a black overlay and suspends the WebView2
        /// so YouTube stops streaming/connecting. Stays off until the user
        /// presses On.
        /// </summary>
        private async void TurnOff()
        {
            this.isOff = true;
            this.blackOverlay.Visibility = Visibility.Visible;
            this.webView.Visibility = Visibility.Collapsed;
            this.btnOff.Content = "On";

            try
            {
                // TrySuspendAsync only succeeds while the control is hidden,
                // which is why it runs after collapsing the WebView2.
                if (this.webView.CoreWebView2 != null)
                {
                    await this.webView.CoreWebView2.TrySuspendAsync();
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Resumes the view from the Off state and shows YouTube again.
        /// </summary>
        private void TurnOn()
        {
            this.isOff = false;
            this.blackOverlay.Visibility = Visibility.Collapsed;
            this.webView.Visibility = Visibility.Visible;
            this.btnOff.Content = "Off";
            this.webView.CoreWebView2?.Resume();
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
