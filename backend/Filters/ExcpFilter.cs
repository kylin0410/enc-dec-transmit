using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Net;

namespace DecBackEnd.Filters
{
    /// <summary>
    /// 例外處理攔截器，於Startup注入。
    /// </summary>
    public class ExcpFilter : ExceptionFilterAttribute
    {
        private readonly ILogger<ExcpFilter> _logger;

        public ExcpFilter(ILogger<ExcpFilter> logger)
        {
            _logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            var ex = context.Exception;
            _logger.LogError($"{ex.ToString()}");

            // 錯誤訊息
            var response = new
            {
                Message = "出現非預期的錯誤",
                Error = ex.ToString()
            };

            // 將例外處理訊息放進filterContext回傳至前端.
            context.Result = new JsonResult(response);
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;  // 500 Server Error.
            context.ExceptionHandled = true;
            base.OnException(context);
        }
    }
}
