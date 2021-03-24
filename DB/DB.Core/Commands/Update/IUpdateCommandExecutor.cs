using DB.Core.State;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;

namespace DB.Core.Commands.Update
{
    public interface IUpdateCommandExecutor
    {
        bool CanExecute(JToken parameters);
        void Execute(IDbState state, string collectionName, string id, ConcurrentDictionary<string, string> document, JToken parameters);
    }
}
