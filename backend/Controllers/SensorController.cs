using Microsoft.AspNetCore.Mvc;
using SensorApi.Models;
using SensorApi.Services;

namespace SensorApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SensorController : ControllerBase
    {
        private readonly ISensorService _sensorService;

        public SensorController(ISensorService sensorService)
        {
            _sensorService = sensorService;
        }

        [HttpPost("receive")]
        public IActionResult Receive([FromBody] SensorData data)
        {
            _sensorService.ProcessData(data);
            return Ok(new { status = "received" });
        }
    }
}
