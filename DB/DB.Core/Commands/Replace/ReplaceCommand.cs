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

            return Result.Ok.Empty;
        }

        private void DeleteDocumentFromIndexes(IDbState state, string collectionName, string id, ConcurrentDictionary<string, string> document)
        {
            if (state.Indexes.TryGetValue(collectionName, out var fields))
            {
                foreach (var kvp in document)
                {
                    if (!fields.TryGetValue(kvp.Key, out var values))
                        continue;
                    if (!values.TryGetValue(kvp.Value, out var documents))
                        continue;
                    documents.Remove(id);
                }
            }
        }

        private void AddDocumentInIndexes(IDbState state, string collectionName, string id, ConcurrentDictionary<string, string> document)
        {
            if (state.Indexes.TryGetValue(collectionName, out var fields))
            {
                foreach (var kvp in document)
                {
                    if (!fields.TryGetValue(kvp.Key, out var values))
                        continue;
                    var documents = values.GetOrAdd(kvp.Value, _ => new List<string>());
                    documents.Add(id);
                }
            }
        }
    }
}