using Confluent.Kafka;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

// Simple console app to simulate a system generating telemetry data
Console.Write("------DataGeneratingProgram------\n\n");

var config = new ProducerConfig
{
    BootstrapServers = "localhost:9092"
};

using (var producer = new ProducerBuilder<Null, string>(config).Build())
{
    for (var i = 0; i < 1000; i++)
    {

        string x = Convert.ToString(new Random().Next(1, 300));
        string y = Convert.ToString(new Random().Next(1, 300));
        string z = Convert.ToString(new Random().Next(1, 300));

        string data = $"{{\"x\": {x}, \"y\": {y}, \"z\": {z}}}";

        var result = await producer.ProduceAsync("TelemetryData", new Message<Null, string> { Value = $"{data}", Timestamp = new Timestamp(DateTimeOffset.Now) });
        Console.WriteLine($"{result.Message.Timestamp.UtcDateTime} : {result.Message.Value}");
        Console.Out.Flush();

        await Task.Delay(20);
    }
}