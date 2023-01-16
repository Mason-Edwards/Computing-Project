namespace Dashboard.Client.Models
{
    /// <summary>
    /// Class to represent a data point to be plotted on the graph.
    /// </summary>
    public class ParameterData
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="value">Value of the datapoint.</param>
        /// <param name="timestamp">Timestamp that data point was created, in milliseconds from unix time.</param>
        public ParameterData(string? value, long? timestamp)
        {
            Value = value;
            Timestamp = timestamp;
        }

        /// <summary>
        /// Timestamp of the data point.
        /// </summary>
        public long? Timestamp { get; set; }
        /// <summary>
        /// Value of the data point.
        /// </summary>
        public string? Value { get; set; }

    }
}
