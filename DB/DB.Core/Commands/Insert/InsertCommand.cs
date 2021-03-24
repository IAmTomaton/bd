using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using DB.Core.Helpers;
using DB.Core.State;
using Newtonsoft.Json.Linq;

namespace DB.Core.Commands.Insert
{
    public class InsertCommand : ICommand
    {
        private readonly IInsertCommandParser parser;

        public InsertCommand(IInsertCommandParser parser)
            => this.parser = parser;

        public string Name => "insert";

        public JObject Execute(IDbState state, JObject parameters)
        {
            var (ok, collectionName, id, Jdocument) = parser.Parse(parameters);

            if (!ok)
            {
                return Result.Error.InvalidRequest;
            }

            var collection = state.Collections.GetOrAdd(collectionName, _ => new ConcurrentDictionary<string, ConcurrentDictionary<string, string>>());

            if (collection.ContainsKey(id))
            {
                return Result.Error.AlreadyExists;
            }

            var document = Jdocument.ToObject<ConcurrentDictionary<string, string>>();
            collection[id] = document;

            AddDocumentInIndexes(state, collectionName, id, document);

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