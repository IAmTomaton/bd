using System.Linq;
using DB.Core.Validation;
using Newtonsoft.Json.Linq;

namespace DB.Core.Commands.Replace
{
    public class ReplaceCommandParser : IReplaceCommandParser
    {
        private readonly IDocumentValidator validator;

        public ReplaceCommandParser(IDocumentValidator validator)
            => this.validator = validator;

        public (bool Ok, string CollectionName, string Id, JObject Document, bool Upsert) Parse(JObject parameters)
        {
            if (parameters.Count != 2)
            {
                return default;
            }

            var upsertProperty = parameters.Properties().First(x => x.Name == "upsert");
            var upsert = upsertProperty.Single().ToObject<bool>();
            
            var collectionProperty = parameters.Properties().First(x => x.Name != "upsert");
            var collectionName = collectionProperty.Name;

            if (collectionProperty.Count != 1)
            {
                return default;
            }

            if (!(collectionProperty.First() is JObject idAndDocument))
            {
                return default;
            }

            var idProperty = idAndDocument.Properties().First();
            var id = idProperty.Name;

            if (idProperty.Count != 1)
            {
                return default;
            }
            
            if (!(idProperty.First() is JObject document))
            {
                return default;
            }

            if (!validator.IsValid(document))
            {
                return default;
            }

            return (true, collectionName, id, document, upsert);
        }
    }
}