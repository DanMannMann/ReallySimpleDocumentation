using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;

namespace Marsman.ReallySimpleDocumentation
{
    public static class ErrorHandlerExtension
    {
        /// <summary>
        /// Causes the debugger to break on unhandled request exceptions, and returns exception details in the response once it resumes. This is useful for diagnosing problems with SwaggerGen or other swagger middleware.
        /// <para>This middleware never does anything unless the debugger is attached.</para>
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseBreakpointErrorMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<BreakpointErrorHandler>();
        }
    }

    public class BreakpointErrorHandler
    {
        private readonly RequestDelegate next;
        private readonly IHostingEnvironment _env;
        private readonly ILogger<BreakpointErrorHandler> _logger;

        public BreakpointErrorHandler(RequestDelegate next, ILogger<BreakpointErrorHandler> logger, IHostingEnvironment env)
        {
            this.next = next;
            _env = env;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }

        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            if (Debugger.IsAttached)
            {
                Debugger.Break();
                var code = HttpStatusCode.InternalServerError;
                var result = JsonConvert.SerializeObject(new { Code = code, Error = exception.Message, exception.StackTrace });
                return context.Response.WriteAsync(result);
            }

            return context.Response.WriteAsync("");

        }
    }
}
