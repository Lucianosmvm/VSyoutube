namespace YouTubeToolbar
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;

    /// <summary>
    /// Tool window that hosts the YouTube user control.
    /// </summary>
    [Guid("dc35a1eb-90a3-429a-9869-795093c00976")]
    public class YouTubeTools : ToolWindowPane
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="YouTubeTools"/> class.
        /// </summary>
        public YouTubeTools() : base(null)
        {
            this.Caption = "YouTube";
            this.Content = new YouTubeToolsControl();
        }
    }
}
