using DB.Core.Helpers;
using DB.Core.State;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace DB.Core.Commands.Indexes
{
    public class AddIndexCommand : ICommand
    {
        public string Name => "addIndex";

        public JObject Execute(IDbState state, JObject parameters)
        {
            if (parameters.Count != 1)
                return Result.Error.InvalidRequest;

            var property = parameters.Properties().First();
            var collectionName = property.Name;

            if (property.Value.Type != JTokenType.String)
                return Result.Error.InvalidRequest;

            var field = property.Value.ToObject<string>();

            state.Indexes.TryAdd(collectionName, new ConcurrentDictionary<string, ConcurrentDictionary<string, List<string>>>());

            if (state.Indexes[collectionName].ContainsKey(field))
                return Result.Error.AlreadyExists;

            state.Indexes[collectionName].TryAdd(field, new ConcurrentDictionary<string, List<string>>());

            if (state.Collections.TryGetValue(collectionName, out var collection))
                foreach (var idDocumentPair in collection)
                    AddDocumentInIndexes(state, collectionName, idDocumentPair.Key, idDocumentPair.Value);

            return Result.Ok.Empty;
        }

        private void AddDocumentInIndexes(IDbState state, string collectionName, string id, ConcurrentDictionary<string, string> document)
        {
            if (state.Indexes.TryGetValue(collectionName, out var fields))
            {
                foreach (var fieldValuePair in document)
                {
                    if (!fields.TryGetValue(fieldValuePair.Key, out var values))
                        continue;
                    var documents = values.GetOrAdd(fieldValuePair.Value, _ => new List<string>());
                    documents.Add(id);
                }
            }
        }
    }
}
