using System.Collections.Concurrent;
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
            var (ok, collectionName, id, document, upsert) = parser.Parse(parameters);

            if (!ok)
            {
                return Result.Error.InvalidRequest;
            }

            var collection = state.Collections.GetOrAdd(collectionName, _ => new ConcurrentDictionary<string, ConcurrentDictionary<string, string>>());

            if (!collection.ContainsKey(id) && !upsert)
            {
                return Result.Error.NotFound;
            }

            collection[id] = document.ToObject<ConcurrentDictionary<string, string>>();
            return Result.Ok.Empty;
        }
    }
}