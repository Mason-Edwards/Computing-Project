namespace Dashboard.Client.Models
{
    /// <summary>
    /// Class to represent a "label" on the chart. Each label will be a different parameter on the chart.
    /// This class will be serialised with the data of a parameter and passed via Javascript interop to the chart
    /// library.
    /// </summary>
    public class Parameter
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="label"> The name of the parameter to be the label on the chart.</param>
        /// <param name="borderWidth"> Width of the line on the chart.</param>
        /// <param name="borderColor"> Color of the line on the chart.</param>
        /// <param name="backgroundColor">Background color of datapoint and label on the chart.</param>
        public Parameter(string? label, int? borderWidth, string? borderColor, string? backgroundColor)
        {
            Label = label;
            BorderWidth = borderWidth;
            BorderColor = borderColor;
            BackgroundColor = backgroundColor;
        }

        /// <summary>
        /// The label on the chart.
        /// </summary>
        public string? Label { get; set; }
        /// <summary>
        /// Width of the line on the chart.
        /// </summary>
        public int? BorderWidth { get; set; }
        /// <summary>
        /// Color of the line on the chart.
        /// </summary>
        public string? BorderColor { get; set; }
        /// <summary>
        /// Background color of datapoint and label on the chart.
        /// </summary>
        public string? BackgroundColor { get; set; }
    }
}
