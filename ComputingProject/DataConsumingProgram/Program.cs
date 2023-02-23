using Confluent.Kafka;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

Console.Write("------DataReadingProgram------\n");
var topic = "TelemetryData";

var config = new ConsumerConfig
{
    BootstrapServers = "localhost:9092",
    GroupId = "TelemetryReader",
    AutoOffsetReset = AutoOffsetReset.Earliest
};

JSchema schemaJson = JSchema.Parse(@"{
            'description': 'Telemetry Data',
            'type': 'object',
            'properties': {
                'parameter': {'type':'string'},
                'unit': {'type' : 'string'},
                'value' : {'type' : 'string'}
            },
            'required' : ['parameter', 'unit', 'value'],
            'additionalProperties' : false
        }");

using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
{
    consumer.Subscribe(new List<string> {topic});
    Console.Write(value: $"Subscribed to {topic}\n\n");


    // Consumer Loop
    while (true)
    {
        // Consume message
        ConsumeResult<Ignore, string>? consumeResult = consumer.Consume();

        // Validate message using JSON schema
        bool valid = JObject.Parse(consumeResult.Message.Value).IsValid(schemaJson);

        if(valid)
        {
            Console.WriteLine($"{consumeResult.Message.Timestamp.UtcDateTime} : {consumeResult.Message.Value}");

            // Write to database

            // GRPC to dashboard

            // This is the gRPC client which will call the methods on the GRPC server and pass the info when it is recieved.
            
            // IMPORTATNT: This logic needs to be moved to grpcServer project as the dashboard will be calling the grpc server for the telemetry data.
        }




        Console.Out.Flush();

        // 
        Task.Delay(5000);
    }
}
