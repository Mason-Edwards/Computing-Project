using Grpc.Core;
using Dashboard.Shared.GrpcProto;
using Confluent.Kafka;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Google.Protobuf.WellKnownTypes;
using static Confluent.Kafka.ConfigPropertyNames;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using System.Reactive;

namespace grpcServer.Services
{
    public class TelemetryDataService : TelemetryData.TelemetryDataBase
    {
        private readonly ILogger<TelemetryDataService> _logger;
        private readonly IInfluxDBClient _influxDbClient;
        private static GrpcRecordingStatus record = GrpcRecordingStatus.NotRecoding;

        // Move to a config file
        private const string topic = "TelemetryData";
        private const string org = "defaultOrg";
        private const string bucket = "defaultBucket";

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
                'value' : {'type' : 'string'},
                'timestamp' : {'type' : 'string'}
            },
            'required' : ['parameter', 'unit', 'value', 'timestamp'],
            'additionalProperties' : false
        }");


        public TelemetryDataService(ILogger<TelemetryDataService> logger, IInfluxDBClient influxDBClient)
        {
            _logger = logger;
            _influxDbClient = influxDBClient;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name
            });
        }

        public override async Task OpenTelemetryStream(Empty empty, IServerStreamWriter<Data> responseStream ,ServerCallContext context)
        {
            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            consumer.Subscribe(new List<string> { topic });

            while (!context.CancellationToken.IsCancellationRequested)
            {
                // Consume message
                ConsumeResult<Ignore, string>? consumeResult = consumer.Consume();

                // Convert message to JSON
                JObject message = JObject.Parse(consumeResult.Message.Value);
                // Validate message using JSON schema
                //bool valid = message.IsValid(schemaJson);

                //if (!valid)
                //{
                    //await responseStream.WriteAsync(new Data { Value = "data not valid" });
                    //continue;
               //}
                Data toSend = new Data
                {
                    // Return message (boolean, gRPC status?)
                    Parameter = message.Property("parameter").Value.ToString(),
                    Unit = message.Property("unit").Value.ToString(),
                    Value = message.Property("value").Value.ToString(),
                    Timestamp = consumeResult.Message.Timestamp.UnixTimestampMs.ToString(),
                };

                if(record == GrpcRecordingStatus.Recording)
                {

                    Console.WriteLine($"WRITING DATA {toSend.ToString()}");
                    using var writeApi = _influxDbClient.GetWriteApi();

                    var datetime = DateTimeOffset.FromUnixTimeMilliseconds(consumeResult.Message.Timestamp.UnixTimestampMs);

                    // Create data point
                    var point = PointData.Measurement(toSend.Parameter)
                    .Tag("unit", toSend.Unit)
                    .Field("value", toSend.Value)
                    .Timestamp(datetime.UtcDateTime, WritePrecision.Ns);

                    // Write to db
                    // Might need to be done async or something to help performance?
                    writeApi.WritePoint(point, bucket, org);
                }

                await responseStream.WriteAsync(toSend);
            }
        }

        public override Task<Empty> RecordTelemetry(RecordTelemetryMessage recordTelemetryMessage, ServerCallContext context)
        {
            record = recordTelemetryMessage.RecordingStatus;
            Console.WriteLine($"CHANGING STATUS TO {record.ToString()}");
            return Task.FromResult(new Empty());
        }

        public override async Task<LoadTelemetryDataReply> LoadTelemetryData(LoadTelemetryDataRequest request, ServerCallContext context) 
        {
            List<Data> datas= new List<Data>();
            var flux = $"from(bucket: \"defaultBucket\") |> range(start: {request.Start}, stop: {request.End})";

            var fluxTables = await _influxDbClient.GetQueryApi().QueryAsync(flux, org);
            fluxTables.ForEach(fluxTable =>
            {
                var fluxRecords = fluxTable.Records;
                fluxRecords.ForEach(fluxRecord =>
                {
                    Console.WriteLine($"{fluxRecord.GetTime()}: {fluxRecord.GetMeasurement()} | {fluxRecord.GetValueByKey("unit")} | {fluxRecord.GetValue()}");
                    var data = new Data() { Timestamp = fluxRecord.GetTime().ToString(), Parameter = fluxRecord.GetMeasurement(), Unit = fluxRecord.GetValueByKey("unit").ToString(), Value = fluxRecord.GetValue().ToString() };
                    datas.Add(data);
                });
            });

            LoadTelemetryDataReply test = new LoadTelemetryDataReply();
            test.DataList.AddRange(datas);

            return test;
        }
    }
}