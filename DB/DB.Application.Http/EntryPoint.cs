using DB.Application.Core;
using DB.Application.Http.Configuration;
using DB.Application.Http.Implementation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Vostok.Logging.Console;
using Vostok.Logging.Formatting;
using Vostok.Logging.Tracing;
using Vostok.Tracing;
using Vostok.Tracing.Abstractions;

await CreateHostBuilder(args)
    .Build()
    .RunAsync()
    .ConfigureAwait(false);

static IHostBuilder CreateHostBuilder(string[] args)
{
    var port = args.Length == 1 ? args[0] : "7012"; 

    var tracer = new Tracer(new TracerSettings(new DevNullSpanSender()));
    var log = new ConsoleLog(new ConsoleLogSettings
    {
        ColorsEnabled = true,
        OutputTemplate = new OutputTemplateBuilder().AddTimestamp().AddLevel().AddMessage().AddException().AddNewline().AddAllProperties().AddNewline().Build()
    }).WithTracingProperties(tracer);
    var server = new Server();

    var application = new Application(server);
    application.Init();

    return new HostBuilder()
        .ConfigureServices(services => services.AddServices(log, tracer, server))
        .ConfigureWebHostDefaults(webHostBuilder => webHostBuilder
            .UseUrls($"http://+:{port}")
            .UseKestrel()
            .ConfigureApplication());
}