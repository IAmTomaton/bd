using System.Threading.Tasks;
using DB.Client.Core;

namespace DB.Tests.Unit
{
    public class TestDbRequestSender : IDbRequestSender
    {
        private readonly TestServer server;

        public TestDbRequestSender(TestServer server)
            => this.server = server;

        public Task<string> SendAsync(string command)
            => server.HandleAsync("/execute", command);
    }
}