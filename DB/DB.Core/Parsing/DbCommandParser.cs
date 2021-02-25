using System.Linq;
using Newtonsoft.Json.Linq;

namespace DB.Core.Parsing
{
    public class DbCommandParser : IDbCommandParser
    {
        public (bool Ok, string CommandName, JObject Parameters) Parse(string input)
        {
            if (!TryParse(input, out var jObject))
            {
                return default;
            }

            if (jObject.Count != 1)
            {
                return default;
            }

            var commandProperty = jObject.Properties().First();
            if (!(commandProperty.First is JObject parameters))
            {
                return default;
            }

            return (true, commandProperty.Name, parameters);
        }

        private static bool TryParse(string json, out JObject jObject)
        {
            try
            {
                jObject = JObject.Parse(json);
                return true;
            }
            catch
            {
                jObject = null;
                return false;
            }
        }
    }
}