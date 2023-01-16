using Grpc.Core;
using Dashboard.Shared.GrpcProto;

namespace grpcServer.Services
{
    public class TelemetryDataService : TelemetryData.TelemetryDataBase
    {
        private readonly ILogger<TelemetryDataService> _logger;
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
    }
}