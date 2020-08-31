using DecBackEnd.DataSchemas;
using DecBackEnd.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DecBackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(LoggingFilter))]
    public class HeartbeatController : ControllerBase
    {
        private readonly ILogger _logger;

        public HeartbeatController(ILogger<HeartbeatController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Heartbeat上傳API
        /// </summary>
        /// <param name="postData"></param>
        // POST api/HwInfo
        [HttpPost]
        [ServiceFilter(typeof(ExcpFilter))]
        public void Post([FromBody] string postData)
        {
            _logger.LogDebug("Got heartbeat data.");
            _logger.LogDebug(postData);
            Heartbeat heartbeat = JsonConvert.DeserializeObject<Heartbeat>(postData);
        }
    }
}
