using Confluent.Kafka;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

// Simple console app to simulate a system generating telemetry data
Console.Write("------DataGeneratingProgram------\n\n");

JSchema schemaJson = JSchema.Parse(@"{
            'description': 'Telemetry Data',
            'type': 'object',
            'properties': {
                'parameter': {'type':'string'},
                'unit': {'type' : 'string'},
                'value' : {'type' : 'string'},
                'timestamp' : {'type' : 'string'}
            },
            'required' : ['parameter', 'unit', 'value', 'timestamp'],
            'additionalProperties' : false
        }");

var config = new ProducerConfig
{
    BootstrapServers = "localhost:9092"
};

using (var producer = new ProducerBuilder<Null, string>(config).Build())
{
    for (var i = 0; i < 1000; i++)
    {
        
        string x = Convert.ToString(new Random().Next(1, 300));
        string data = $"{{\"parameter\": \"Test parameter\", \"unit\": \"g\", \"value\": \"{x}\", \"timestamp\": \"{new Timestamp(DateTimeOffset.Now).UtcDateTime}\" }}";

        JObject dataToSend = JObject.Parse(data);

        bool valid = dataToSend.IsValid(schemaJson);

        string message = dataToSend.ToString();

        var result = await producer.ProduceAsync("TelemetryData", new Message<Null, string> { Value = $"{dataToSend.ToString()}", Timestamp = new Timestamp(DateTimeOffset.Now) });
        Console.WriteLine($"{result.Message.Timestamp.UtcDateTime} : {result.Message.Value}");
        Console.Out.Flush();

        await Task.Delay(1000);
    }
}