using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DecBackEnd.Filters
{
    /// <summary>
    /// Log input and output message for every API calls.
    /// </summary>
    public class LoggingFilter : IActionFilter
    {
        private ILogger _logger { get; set; }

        public LoggingFilter(ILogger<LoggingFilter> logger)
        {
            this._logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var formData = JsonConvert.SerializeObject(context.ActionArguments);
            this._logger.LogDebug(string.Format("OnActionExcuting | {0} ", formData));
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            var result = string.Empty;
            if (context.Result is JsonResult)
            {
                result = JsonConvert.SerializeObject((context.Result as JsonResult).Value);
            }
            if (context.Exception != null)
            {
                result = context.Exception.Message;
            }
            this._logger.LogDebug(string.Format("OnActionExcuted | {0} ", result));
        }
    }
}
