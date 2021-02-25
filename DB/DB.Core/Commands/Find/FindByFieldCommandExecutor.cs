using System;
using System.Collections.Concurrent;
using System.Linq;
using DB.Core.Helpers;
using DB.Core.State;
using Newtonsoft.Json.Linq;

namespace DB.Core.Commands.Find
{
    public class FindByFieldCommandExecutor : IFindCommandExecutor
    {
        public bool CanExecute(JToken parameters)
            => parameters is JObject {Count: 1} jObject
               && jObject.Properties().Single().Value.Type == JTokenType.String;

        public JObject Execute(IDbState state, string collectionName, JToken parameters)
        {
            var property = ((JObject) parameters).Properties().Single();

            var field = property.Name;
            var value = property.Value.ToObject<string>();
            
            if (!state.Collections.TryGetValue(collectionName, out var collection))
            {
                return Result.Ok.WithContent(Array.Empty<object>());
            }

            return Result.Ok.WithContent(
                collection.Where(document => document.Value.TryGetValue(field, out var docValue) && docValue == value)
                    .Select(kvp => GetJObject(kvp.Key, kvp.Value))
            );
        }

        private static JObject GetJObject(string id, ConcurrentDictionary<string, string> document)
            => new(
                new JProperty(id,
                    new JObject(document.Select(kvp => new JProperty(kvp.Key, kvp.Value)))));
    }
}