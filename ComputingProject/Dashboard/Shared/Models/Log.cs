using System;

namespace Dashboard.Shared.Models
{
    public class Log
    {
        public Log(Parameter parameter, string log, DateTimeOffset timestamp)
        {
            Parameter = parameter;
            LogMessage = log;
            Timestamp = timestamp;
        }

        public Parameter Parameter { get; set;}

        public string LogMessage { get; set; }
            
        public DateTimeOffset Timestamp { get; set; }
    }
}
