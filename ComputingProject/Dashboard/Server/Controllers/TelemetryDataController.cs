// gRPC client namespace is defined in the proto file
using dashboardGrpcClient;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Dashboard.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TelemetryDataController : ControllerBase
    {

        private readonly ILogger<TelemetryDataController> _logger;
        private readonly Greeter.GreeterClient _client;

        public TelemetryDataController(ILogger<TelemetryDataController> logger, Greeter.GreeterClient client)
        {
            _logger = logger;
            _client = client;
        }

        [HttpGet]
        public string Get()
        {
            var test = _client.SayHello(new HelloRequest { Name = "gRPC test." });
            return JsonSerializer.Serialize(test.Message);
        }
    }
}