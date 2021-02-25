using System;
using System.Threading.Tasks;
using DB.Client.Core;
using Vostok.Clusterclient.Core;
using Vostok.Clusterclient.Core.Model;
using Vostok.Clusterclient.Tracing;
using Vostok.Clusterclient.Transport;
using Vostok.Logging.Abstractions;
using Vostok.Tracing.Abstractions;

namespace DB.Client.Http
{
    public class DbRequestSender : IDbRequestSender
    {
        private readonly IClusterClient clusterClient;

        public DbRequestSender(Uri uri, ILog log = null, ITracer tracer = null)
            => clusterClient = new ClusterClient(
                (log ?? LogProvider.Get()).ForContext<DbRequestSender>(),
                config =>
                {
                    config.Logging.LogReplicaRequests = false;
                    config.Logging.LogReplicaResults = false;

                    config.SetupExternalUrl(uri);
                    config.SetupUniversalTransport();
                    config.SetupDistributedTracing(tracer ?? TracerProvider.Get());
                });

        public async Task<string> SendAsync(string command)
        {
            var request = Request.Post("/execute").WithContent(command);
            var result = await clusterClient.SendAsync(request).ConfigureAwait(false);
            result.Response.EnsureSuccessStatusCode();
            return result.Response.Content.ToString();
        }

        public async Task PingAsync()
        {
            var request = Request.Post("/ping");
            var result = await clusterClient.SendAsync(request).ConfigureAwait(false);

            result.Response.EnsureSuccessStatusCode();

            if (result.Response.Content.ToString() != "pong")
            {
                throw new Exception("Ping failed");
            }
        }
    }
}