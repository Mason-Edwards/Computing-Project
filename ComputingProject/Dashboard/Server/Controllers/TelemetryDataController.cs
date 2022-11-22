using Dashboard.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Dashboard.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TelemetryDataController : ControllerBase
    {

        private readonly ILogger<TelemetryDataController> _logger;

        public TelemetryDataController(ILogger<TelemetryDataController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public string Get()
        {
            return JsonSerializer.Serialize("TEST"); ;
        }
    }
}