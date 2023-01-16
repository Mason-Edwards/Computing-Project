namespace Dashboard.Client.Models
{
    public class ParameterValue
    {
        public ParameterValue(string? value, DateTimeOffset? timestamp)
        {
            Value = value;
            Timestamp = timestamp;
        }

        public string? Value { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
    }
}
