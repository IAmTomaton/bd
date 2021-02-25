using Newtonsoft.Json.Linq;

namespace DB.Core.Commands.Insert
{
    public interface IInsertCommandParser
    {
        (bool Ok, string CollectionName, string Id, JObject Document) Parse(JObject parameters);
    }
}