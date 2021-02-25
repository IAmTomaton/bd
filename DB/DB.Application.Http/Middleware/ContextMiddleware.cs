using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Vostok.Context;

namespace DB.Application.Http.Middleware
{
    public class ContextMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            FlowingContext.RestoreDistributedGlobals(context.Request.Headers["Context-Globals"]);
            await next(context).ConfigureAwait(false);
        }
    }
}