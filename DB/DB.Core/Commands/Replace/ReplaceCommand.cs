using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using DB.Core.Helpers;
using DB.Core.State;
using Newtonsoft.Json.Linq;

namespace DB.Core.Commands.Replace
{
    public class ReplaceCommand : ICommand
    {
        private readonly IReplaceCommandParser parser;

        public ReplaceCommand(IReplaceCommandParser parser)
            => this.parser = parser;

        public string Name => "replace";

        public JObject Execute(IDbState state, JObject parameters)
        {
            var (ok, collectionName, id, Jdocument, upsert) = parser.Parse(parameters);

            if (!ok)
            {
                return Result.Error.InvalidRequest;
            }

            var collection = state.Collections.GetOrAdd(collectionName, _ => new ConcurrentDictionary<string, ConcurrentDictionary<string, string>>());

            if (!collection.ContainsKey(id) && !upsert)
            {
                return Result.Error.NotFound;
            }

            if (collection.TryGetValue(id, out var oldDocument))
            {
                DeleteDocumentFromIndexes(state, collectionName, id, oldDocument);
            }

            var document = Jdocument.ToObject<ConcurrentDictionary<string, string>>();
            collection[id] = document;

            AddDocumentInIndexes(state, collectionName, id, document);

            throw new Exception("eba r");

            return Result.Ok.Empty;
        }

        private void DeleteDocumentFromIndexes(IDbState state, string collectionName, string id, ConcurrentDictionary<string, string> document)
        {
            if (state.Indexes.TryGetValue(collectionName, out var fields))
            {
                foreach (var fieldValuePair in document)
                {
                    if (!fields.TryGetValue(fieldValuePair.Key, out var valuesDocuments))
                        continue;

                    var values = valuesDocuments.Item1;
                    var documents = valuesDocuments.Item2;

                    var indexToRemove = documents.FindIndex(docId => docId == id);
                    values.RemoveAt(indexToRemove);
                    documents.RemoveAt(indexToRemove);
                }
            }
        }

        private void AddDocumentInIndexes(IDbState state, string collectionName, string id, ConcurrentDictionary<string, string> document)
        {
            if (state.Indexes.TryGetValue(collectionName, out var fields))
            {
                foreach (var fieldValuePair in document)
                {
                    if (!document.TryGetValue(fieldValuePair.Key, out var value))
                        continue;
                    if (!fields.TryGetValue(fieldValuePair.Key, out var valuesDocuments))
                        continue;

                    var values = valuesDocuments.Item1;
                    var documents = valuesDocuments.Item2;

                    var indexToAdd = values.FindIndex(docValue => docValue == value);
                    values.Insert(indexToAdd, value);
                    documents.Insert(indexToAdd, id);
                }
            }
        }
    }
}