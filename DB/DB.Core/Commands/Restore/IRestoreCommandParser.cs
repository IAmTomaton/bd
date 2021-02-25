using Newtonsoft.Json.Linq;

namespace DB.Core.Commands.Restore
{
    public interface IRestoreCommandParser
    {
        (bool Ok, JObject Collections) Parse(JObject parameters);
    }
}
