using Newtonsoft.Json.Linq;

namespace DB.Core.Parsing
{
    public interface IDbCommandParser
    {
        public (bool Ok, string CommandName, JObject Parameters) Parse(string input);
    }
}