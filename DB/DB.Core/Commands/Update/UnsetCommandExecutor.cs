using DB.Core.Helpers;
using DB.Core.State;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;
using System.Linq;

namespace DB.Core.Commands.Update
{
    public class UnsetCommandExecutor : IUpdateCommandExecutor
    {
        public bool CanExecute(JToken parameters) =>
            parameters is JObject { Count: 1 } jObject &&
            jObject.Properties().Single().Name == "unset" &&
            jObject.Properties().Single().Value.Type == JTokenType.String;

        public void Execute(IDbState state, string collectionName, string id, ConcurrentDictionary<string, string> document, JToken parameters)
        {
            var property = ((JObject)parameters).Properties().Single();

            var field = property.Value.ToObject<string>();

            document.TryRemove(field, out var _);
        }
    }
}
