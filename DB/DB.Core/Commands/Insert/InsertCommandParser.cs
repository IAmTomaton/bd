using System.Linq;
using DB.Core.Validation;
using Newtonsoft.Json.Linq;

namespace DB.Core.Commands.Insert
{
    public class InsertCommandParser : IInsertCommandParser
    {
        private readonly IDocumentValidator validator;

        public InsertCommandParser(IDocumentValidator validator)
            => this.validator = validator;

        public (bool Ok, string CollectionName, string Id, JObject Document) Parse(JObject parameters)
        {
            if (parameters.Count != 1)
            {
                return default;
            }

            var collectionProperty = parameters.Properties().First();
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

            return (true, collectionName, id, document);
        }
    }
}