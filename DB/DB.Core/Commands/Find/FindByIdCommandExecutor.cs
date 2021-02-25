using System.Collections.Concurrent;
using System.Linq;
using DB.Core.Helpers;
using DB.Core.State;
using Newtonsoft.Json.Linq;

namespace DB.Core.Commands.Find
{
    public class FindByIdCommandExecutor : IFindCommandExecutor
    {
        public bool CanExecute(JToken parameters)
            => parameters.Type == JTokenType.String;

        public JObject Execute(IDbState state, string collectionName, JToken parameters)
        {
            var id = parameters.ToObject<string>();
            
            if (!state.Collections.TryGetValue(collectionName, out var collection))
            {
                return Result.Error.NotFound;
            }

            if (!collection.ContainsKey(id))
            {
                return Result.Error.NotFound;
            }
            
            return Result.Ok.WithContent(GetJObject(id, collection[id]));
        }
        
        private static JObject GetJObject(string id, ConcurrentDictionary<string, string> document)
            => new (
                new JProperty(id,
                    new JObject(document.Select(kvp => new JProperty(kvp.Key, kvp.Value)))));
    }
}