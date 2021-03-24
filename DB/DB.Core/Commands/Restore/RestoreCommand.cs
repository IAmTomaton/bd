using DB.Core.Helpers;
using DB.Core.State;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace DB.Core.Commands.Restore
{
    public class RestoreCommand : ICommand
    {
        private readonly IRestoreCommandParser parser;

        public RestoreCommand(IRestoreCommandParser parser)
            => this.parser = parser;

        public string Name => "restore";

        public JObject Execute(IDbState state, JObject parameters)
        {
            var (ok, tables) = parser.Parse(parameters);

            if (!ok)
            {
                return Result.Error.InvalidRequest;
            }

            state.Collections.Clear();
            foreach (var keyTablePair in tables)
            {
                state.Collections[keyTablePair.Key] = keyTablePair.Value;
                if (state.Indexes.TryGetValue(keyTablePair.Key, out var tableIndexes))
                {
                    AddDocumentsInIndexes(tableIndexes, keyTablePair.Value);
                }
            }

            return Result.Ok.Empty;
        }

        private void AddDocumentsInIndexes(ConcurrentDictionary<string, ConcurrentDictionary<string, List<string>>> tableIndexes,
            ConcurrentDictionary<string, ConcurrentDictionary<string, string>> documents)
        {
            foreach (var document in documents)
            {
                foreach (var fieldValuePair in document.Value)
                {
                    if (tableIndexes.TryGetValue(fieldValuePair.Key, out var values))
                    {
                        var ids = values.GetOrAdd(fieldValuePair.Value, _ => new List<string>());
                        ids.Add(document.Key);
                    }
                }
            }
        }
    }
}
