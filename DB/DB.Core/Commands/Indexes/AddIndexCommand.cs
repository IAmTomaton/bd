using DB.Core.Helpers;
using DB.Core.State;
using Newtonsoft.Json.Linq;
using System;
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

            var collectionIndexes = state.Indexes.GetOrAdd(collectionName, new ConcurrentDictionary<string, Tuple<List<string>, List<string>>>());

            if (collectionIndexes.ContainsKey(field))
                return Result.Error.AlreadyExists;

            collectionIndexes.TryAdd(field, new Tuple<List<string>, List<string>>(new List<string>(), new List<string>()));

            if (state.Collections.TryGetValue(collectionName, out var collection))
                foreach (var idDocumentPair in collection)
                    AddDocumentInIndex(state, collectionName, idDocumentPair.Key, idDocumentPair.Value, field);

            return Result.Ok.Empty;
        }

        private void AddDocumentInIndex(IDbState state, string collectionName, string id, ConcurrentDictionary<string, string> document, string field)
        {
            if (!document.TryGetValue(field, out var value))
                return;
            if (!state.Indexes.TryGetValue(collectionName, out var fields))
                return;
            if (!fields.TryGetValue(collectionName, out var valuesDocuments))
                return;

            var values = valuesDocuments.Item1;
            var documents = valuesDocuments.Item2;

            var indexToAdd = values.FindIndex(v => v == value);
            values.Insert(indexToAdd, value);
            documents.Insert(indexToAdd, id);
        }
    }
}
