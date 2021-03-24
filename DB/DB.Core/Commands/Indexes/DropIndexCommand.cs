using DB.Core.Helpers;
using DB.Core.State;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace DB.Core.Commands.Indexes
{
    public class DropIndexCommand : ICommand
    {
        public string Name => "dropIndex";

        public JObject Execute(IDbState state, JObject parameters)
        {
            if (parameters.Count != 1)
                return Result.Error.InvalidRequest;

            var property = parameters.Properties().First();
            var collectionName = property.Name;

            if (property.Value.Type != JTokenType.String)
                return Result.Error.InvalidRequest;

            var fieldName = property.Value.ToObject<string>();

            if (!state.Indexes.TryGetValue(collectionName, out var collection))
                return Result.Error.NotFound;

            collection.TryRemove(fieldName, out var _);

            return Result.Ok.Empty;
        }
    }
}
