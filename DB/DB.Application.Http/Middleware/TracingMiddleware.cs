using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Vostok.Tracing.Abstractions;

namespace DB.Application.Http.Middleware
{
    public class TracingMiddleware : IMiddleware
    {
        private readonly ITracer tracer;

        public TracingMiddleware(ITracer tracer)
            => this.tracer = tracer;

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            using var spanBuilder = tracer.BeginSpan();
            await next(context).ConfigureAwait(false);
        }
    }
}