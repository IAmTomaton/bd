using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;

namespace DB.Core.Commands.Restore
{
    public interface IRestoreCommandParser
    {
        (bool Ok, ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentDictionary<string, string>>> Collections) Parse(JObject parameters);
    }
}
