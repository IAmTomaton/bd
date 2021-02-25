using System.Threading.Tasks;
using DB.Core;

namespace DB.Application.Core
{
    public class Application
    {
        private readonly IDb db;
        private readonly IServer server;

        public Application(IServer server)
        {
            this.server = server;
            db = DbFactory.Create();
        }

        public void Init()
        {
            server.AddHandler("/execute", b => Task.FromResult(db.Execute(b)));
        }
    }
}