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

            collection.TryRemove(id, out _);
            return Result.Ok.Empty;
        }
    }
}