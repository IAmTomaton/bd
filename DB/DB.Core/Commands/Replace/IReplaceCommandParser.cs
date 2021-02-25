using Newtonsoft.Json.Linq;

namespace DB.Core.Commands.Replace
{
    public interface IReplaceCommandParser
    {
        (bool Ok, string CollectionName, string Id, JObject Document, bool Upsert) Parse(JObject parameters);
    }
}