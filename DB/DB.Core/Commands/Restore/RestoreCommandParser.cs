using DB.Core.Validation;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;
using System.Linq;

namespace DB.Core.Commands.Restore
{
    public class RestoreCommandParser : IRestoreCommandParser
    {
        private readonly IDocumentValidator validator;

        public RestoreCommandParser(IDocumentValidator validator)
            => this.validator = validator;

        public (bool Ok, ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentDictionary<string, string>>> Collections) Parse(JObject parameters)
        {
            var tablesProperty = parameters.Properties();
            var tables = new ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentDictionary<string, string>>>();

            foreach (var tableProperty in tablesProperty)
            {
                if (!(tableProperty.Value is JArray Cells))
                    return default;
                var table = tables.GetOrAdd(tableProperty.Name, _ => new ConcurrentDictionary<string, ConcurrentDictionary<string, string>>());
                foreach (var cell in Cells.Children())
                {
                    if (!(cell is JObject idAndDocument))
                        return default;
                    if (idAndDocument.Count != 1)
                        return default;

                    var documentProperty = idAndDocument.Properties().First();
                    if (!(documentProperty.Value is JObject document))
                        return default;
                    if (!validator.IsValid(document))
                        return default;

                    table[documentProperty.Name] = document.ToObject<ConcurrentDictionary<string, string>>();
                }
            }

            return (true, tables);
        }
    }
}
