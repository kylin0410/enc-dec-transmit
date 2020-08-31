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
    public class PerformanceController : ControllerBase
    {
        private readonly ILogger _logger;

        public PerformanceController(ILogger<PerformanceController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 修改硬體資訊
        /// </summary>
        /// <param name="postData"></param>
        // POST api/HwInfo
        [ServiceFilter(typeof(ExcpFilter))]
        [HttpPost]
        public void Post([FromBody]string postData)
        {
            _logger.LogDebug("Got performance data.");
            _logger.LogDebug(postData);
            PerformanceData perfData = JsonConvert.DeserializeObject<PerformanceData>(postData);
        }
    }
}
