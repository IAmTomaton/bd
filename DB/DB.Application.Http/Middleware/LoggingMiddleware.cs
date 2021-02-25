using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Vostok.Logging.Abstractions;

namespace DB.Application.Http.Middleware
{
    public class LoggingMiddleware : IMiddleware
    {
        private readonly ILog log;

        public LoggingMiddleware(ILog log)
            => this.log = log.ForContext<LoggingMiddleware>();

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context).ConfigureAwait(false);
            }
            finally
            {
                log.Info("Request {Path} completed with {Code}",
                    context.Request.Path,
                    context.Response.StatusCode);
            }
        }
    }
}