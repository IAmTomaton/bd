using System;
using System.Collections.Concurrent;
using System.Linq;
using DB.Core.Helpers;
using DB.Core.State;
using Newtonsoft.Json.Linq;

namespace DB.Core.Commands.Delete
{
    public class DeleteCommand : ICommand
    {
        public string Name => "delete";

        public JObject Execute(IDbState state, JObject parameters)
        {
            if (parameters.Count != 1)
            {
                return Result.Error.InvalidRequest;
            }

            var property = parameters.Properties().First();
            var collectionName = property.Name;

            if (property.Value.Type != JTokenType.String)
            {
                return Result.Error.InvalidRequest;
            }

            var id = property.Value.ToObject<string>();

            if (!state.Collections.TryGetValue(collectionName, out var collection))
            {
                return Result.Error.NotFound;
            }

            if (!collection.ContainsKey(id))
            {
                return Result.Error.NotFound;
            }

            collection.TryRemove(id, out var document);

            DeleteDocumentFromIndexes(state, collectionName, id, document);

            return Result.Ok.Empty;
        }

        private void DeleteDocumentFromIndexes(IDbState state, string collectionName, string id, ConcurrentDictionary<string, string> document)
        {
            if (state.Indexes.TryGetValue(collectionName, out var fields))
            {
                foreach (var kvp in document)
                {
                    if (!fields.TryGetValue(kvp.Key, out var valuesDocuments))
                        continue;

                    var values = valuesDocuments.Item1;
                    var documents = valuesDocuments.Item2;

                    var indexToDelete = documents.BinarySearch(id);
                    values.RemoveAt(indexToDelete);
                    documents.RemoveAt(indexToDelete);
                }
            }
        }
    }
}