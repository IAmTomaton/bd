using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Vostok.Logging.Abstractions;

namespace DB.Application.Http.Middleware
{
    public class ServerMiddleware : IMiddleware
    {
        private readonly Implementation.Server server;
        private readonly ILog log;

        public ServerMiddleware(Implementation.Server server, ILog log)
        {
            this.server = server;
            this.log = log;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                var body = await new StreamReader(context.Request.Body, Encoding.UTF8).ReadToEndAsync().ConfigureAwait(false);

                var response = await server.HandleAsync(
                        context.Request.Path.Value,
                        body)
                    .ConfigureAwait(false);

                context.Response.StatusCode = StatusCodes.Status200OK;
                await context.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(response)).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                log.Error(e);
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(e.Message)).ConfigureAwait(false);
            }
        }
    }
}