using Newtonsoft.Json.Linq;
using System.Linq;

namespace DB.Core.Commands.Update
{
    public class UpdateCommandParser : IUpdateCommandParser
    {
        public (bool Ok, string CollectionName, string Id, JArray Commands) Parse(JObject parameters)
        {
            if (parameters.Count != 1)
                return default;

            var collectionProperty = parameters.Properties().First();
            var collectionName = collectionProperty.Name;

            if (!(collectionProperty.Value is JObject collection))
                return default;

            if (collection.Count != 1)
                return default;

            var idProperty = collection.Properties().First();
            var id = idProperty.Name;

            if (!(idProperty.Value is JArray commands))
                return default;

            foreach (var commandToken in commands)
            {
                if (!(commandToken is JObject command))
                    return default;
                if (command.Count != 1)
                    return default;
            }
            

            return (true, collectionName, id, commands);
        }
    }
}
