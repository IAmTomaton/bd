using DB.Application.Http.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace DB.Application.Http.Configuration
{
    public static class WebHostBuilderExtensions
    {
        public static IWebHostBuilder ConfigureApplication(this IWebHostBuilder webHostBuilder)
            => webHostBuilder
                .Configure(app => app
                    .UseMiddleware<ContextMiddleware>()
                    .UseMiddleware<TracingMiddleware>()
                    .UseMiddleware<LoggingMiddleware>()
                    .UseMiddleware<ServerMiddleware>())
                .ConfigureServices(services => services
                    .AddTransient<ContextMiddleware>()
                    .AddTransient<TracingMiddleware>()
                    .AddTransient<LoggingMiddleware>()
                    .AddTransient<ServerMiddleware>());
    }
}