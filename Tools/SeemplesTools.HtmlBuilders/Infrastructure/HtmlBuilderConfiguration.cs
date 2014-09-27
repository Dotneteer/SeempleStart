namespace SeemplesTools.HtmlBuilders.Infrastructure
{
    /// <summary>
    /// This static class stores the configuration information used
    /// by the current app to render HTML output
    /// </summary>
    public static class HtmlBuilderConfiguration
    {
        // --- The number of spaces used for indentation
        private static int s_IndentWidth;

        /// <summary>
        /// Indicates whether line breaks should be used or not.
        /// </summary>
        /// <remarks>If line breaks are not used, indentation is also turned off</remarks>
        public static bool UseLineBreaks { get; set; }

        /// <summary>
        /// Gets or sets the current indentation
        /// </summary>
        public static int IndentWidth
        {
            get { return s_IndentWidth; }
            set
            {
                s_IndentWidth = value > 0 ? ( value > 12 ? 12: value) : 0;
            }
        }

        /// <summary>
        /// Gets or sets the character used for indentation
        /// </summary>
        public static char IndentChar { get; set; }

        /// <summary>
        /// Initializes the static members of this class
        /// </summary>
        static HtmlBuilderConfiguration()
        {
            UseLineBreaks = true;
            IndentWidth = 4;
            IndentChar = ' ';
        }
    }
}