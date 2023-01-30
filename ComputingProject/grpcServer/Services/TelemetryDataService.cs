using Grpc.Core;
using Dashboard.Shared.GrpcProto;
using Confluent.Kafka;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Google.Protobuf.WellKnownTypes;
using static Confluent.Kafka.ConfigPropertyNames;

namespace grpcServer.Services
{
    public class TelemetryDataService : TelemetryData.TelemetryDataBase
    {
        private readonly ILogger<TelemetryDataService> _logger;

        private const string topic = "TelemetryData";

        static ConsumerConfig config = new ConsumerConfig
        {
            BootstrapServers = "localhost:9092",
            GroupId = "TelemetryReader",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        static JSchema schemaJson = JSchema.Parse(@"{
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



        // Dont forget to dispose when its been done with.
        // Also create own dispose metod for this service.

        public TelemetryDataService(ILogger<TelemetryDataService> logger)
        {
            _logger = logger;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name
            });
        }

        // Start telemetry method
        // Check that its not running already (already subbed to topic etc)
        public override Task<Reply> StartTelemetry(Empty empty, ServerCallContext context) 
        {

            using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
            {
                while (!context.CancellationToken.IsCancellationRequested)
                {
                    // Consume message
                    ConsumeResult<Ignore, string>? consumeResult = consumer.Consume();

                    // Convert message to JSON
                    JObject message = JObject.Parse(consumeResult.Message.Value);
                    // Validate message using JSON schema
                    bool valid = message.IsValid(schemaJson);

                    if (valid)
                    {
                        Data toSend = new Data
                        {
                            // Return message (boolean, gRPC status?)
                            Parameter = message.Property("parameter").Value.ToString(),
                            Unit = message.Property("unit").Value.ToString(),
                            Value = message.Property("value").Value.ToString()
                        };

                        responseStream.WriteAsync(toSend);
                    }

                    responseStream.WriteAsync(new Data { Value = "data not valid" });

                }
                return Task.FromResult(new Data
                {
                    Value = "end of stream"
                });
            }
        }
        // Restart method
        public override Task<Reply> RestartTelemetry(Empty empty, ServerCallContext context)
        {
            return Task.FromResult(new Reply
            {
                // Return message (boolean, gRPC status?)
                Message = "test"
            });
        }
        // Stop method
        public override Task<Reply> StopTelemetry(Empty empty, ServerCallContext context)
        {
            return Task.FromResult(new Reply
            {
                // Return message (boolean, gRPC status?)
                Message = "test"
            });
        }

        // Get data 
        public override Task<Data> OpenTelemetryStream(Empty empty, IServerStreamWriter<Data> responseStream ,ServerCallContext context)
        {

            using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
            {
                consumer.Subscribe(new List<string> { topic });

                while (!context.CancellationToken.IsCancellationRequested)
                {
                    // Consume message
                    ConsumeResult<Ignore, string>? consumeResult = consumer.Consume();

                    // Convert message to JSON
                    JObject message = JObject.Parse(consumeResult.Message.Value);
                    // Validate message using JSON schema
                    bool valid = message.IsValid(schemaJson);

                    if (valid)
                    {
                        Data toSend = new Data
                        {
                            // Return message (boolean, gRPC status?)
                            Parameter = message.Property("parameter").Value.ToString(),
                            Unit = message.Property("unit").Value.ToString(),
                            Value = message.Property("value").Value.ToString()
                        };

                        responseStream.WriteAsync(toSend);
                    }

                    responseStream.WriteAsync(new Data { Value = "data not valid" });

                }
                return Task.FromResult(new Data
                {
                    Value = "end of stream"
                });
            }
        }
    }
}