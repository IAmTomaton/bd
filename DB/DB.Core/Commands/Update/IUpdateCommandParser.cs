using Newtonsoft.Json.Linq;

namespace DB.Core.Commands.Update
{
    public interface IUpdateCommandParser
    {
        (bool Ok, string CollectionName, string Id, JArray Commands) Parse(JObject parameters);
    }
}
