using DB.Core.State;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace DB.Core.Commands.Update
{
    public class SetCommandExecutor : IUpdateCommandExecutor
    {
        public bool CanExecute(JToken parameters) =>
            parameters is JObject { Count: 1 } jObjectCommand &&
            jObjectCommand.Properties().Single().Name == "set" &&
            jObjectCommand.Properties().Single().Value is JObject { Count: 1 } jObject &&
            jObject.Properties().Single().Value.Type == JTokenType.String;

        public void Execute(IDbState state, string collectionName, string id, ConcurrentDictionary<string, string> document, JToken parameters)
        {
            var commandProperty = ((JObject)parameters).Properties().Single();
            var bodyProperty = ((JObject)commandProperty.Value).Properties().Single();

            var field = bodyProperty.Name;
            var value = bodyProperty.Value.ToObject<string>();

            DeleteFromIndex(state, collectionName, id, field, document);

            document[field] = value;

            AddToIndex(state, collectionName, id, field, document);
        }

        private void DeleteFromIndex(IDbState state, string collectionName, string id, string field, ConcurrentDictionary<string, string> document)
        {
            if (state.Indexes.TryGetValue(collectionName, out var fields))
            {
                if (!fields.TryGetValue(field, out var values))
                    return;
                if (!values.TryGetValue(document[field], out var documents))
                    return;
                documents.Remove(id);
            }
        }

        private void AddToIndex(IDbState state, string collectionName, string id, string field, ConcurrentDictionary<string, string> document)
        {
            if (state.Indexes.TryGetValue(collectionName, out var fields))
            {
                if (!fields.TryGetValue(field, out var values))
                    return;
                var documents = values.GetOrAdd(document[field], _ => new List<string>());
                documents.Add(id);
            }
        }
    }
}
