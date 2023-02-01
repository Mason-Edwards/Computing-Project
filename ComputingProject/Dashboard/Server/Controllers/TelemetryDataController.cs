// gRPC client namespace is defined in the proto file
using Dashboard.Shared.GrpcProto;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Dashboard.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TelemetryDataController : ControllerBase
    {

        private readonly ILogger<TelemetryDataController> _logger;
        private readonly TelemetryData.TelemetryDataClient _client;

        public TelemetryDataController(ILogger<TelemetryDataController> logger, TelemetryData.TelemetryDataClient client)
        {
            _logger = logger;
            _client = client;
        }

        [HttpGet]
        public string Get()
        {
            var test = _client.SayHello(new HelloRequest { Name = "gRPC test." });
            return JsonSerializer.Serialize(test.Message);

            //
        }
    }
}