using System;
using System.Threading;
using DB.Client.Http;
using Vostok.Logging.File;
using Vostok.Logging.File.Configuration;
using Vostok.Logging.Formatting;
using Vostok.Logging.Tracing;
using Vostok.Tracing;
using Vostok.Tracing.Abstractions;

namespace DB.Shell
{
    public static class DbShellFactory
    {
        public static (DbRequestSender Sender, InputBuffer InputBuffer, CancellationToken CancellationToken) Create(string[] args)
        {
            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, _) => cts.Cancel();
            
            var sender = CreateDbRequestSender(args);
            
            var inputBuffer = new InputBuffer();

            return (sender, inputBuffer, cts.Token);
        }
        
        private static DbRequestSender CreateDbRequestSender(string[] args)
        {
            var port = args.Length == 1 ? args[0] : "7012";

            var tracer = new Tracer(new TracerSettings(new DevNullSpanSender()));
            var outputTemplate = new OutputTemplateBuilder().AddTimestamp().AddLevel().AddMessage().AddException().AddNewline().AddAllProperties().AddNewline().Build();
            var log = new FileLog(new FileLogSettings
            {
                FilePath = @"Logs\{RollingPeriod}.log",
                OutputTemplate = outputTemplate,
                RollingStrategy = new RollingStrategyOptions
                {
                    Period = RollingPeriod.Hour,
                    Type = RollingStrategyType.ByTime,
                    MaxFiles = 0
                }
            }).WithTracingProperties(tracer);
            var dbRequestSender = new DbRequestSender(new Uri($"http://localhost:{port}"), log, tracer);
            return dbRequestSender;
        }
    }
}