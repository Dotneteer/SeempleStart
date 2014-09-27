namespace SeemplesTools.HtmlBuilders.Forms
{
    /// <summary>
    /// This interface represents the behavior of a Bootstrap form.
    /// </summary>
    public interface IBsForm
    {
        /// <summary>
        /// Label width with extra small device
        /// </summary>
        int? LabelWidthXs { get; }

        /// <summary>
        /// Label width with small device
        /// </summary>
        int? LabelWidthSm { get; }

        /// <summary>
        /// Label width with medium device
        /// </summary>
        int? LabelWidthMd { get; }

        /// <summary>
        /// Label width with large device
        /// </summary>
        int? LabelWidthLg { get; }

        /// <summary>
        /// Control width with extra small device
        /// </summary>
        int? InputWidthXs { get; }

        /// <summary>
        /// Control width with small device
        /// </summary>
        int? InputWidthSm { get; }

        /// <summary>
        /// Control with with medium device
        /// </summary>
        int? InputWidthMd { get; }

        /// <summary>
        /// Control width with large device
        /// </summary>
        int? InputWidthLg { get; }

        /// <summary>
        /// Name of the form
        /// </summary>
        string FormName { get; }

        /// <summary>
        /// Indicates whether this is a horizontal form
        /// </summary>
        bool IsHorizontal { get; }

        /// <summary>
        /// Indicates whether this is an inline form
        /// </summary>
        bool IsInline { get; }
    }
}