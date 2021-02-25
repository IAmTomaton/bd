using DB.Core.Validation;
using Newtonsoft.Json.Linq;

namespace DB.Core.Commands.Restore
{
    public class RestoreCommandParser : IRestoreCommandParser
    {
        private readonly IDocumentValidator validator;

        public RestoreCommandParser(IDocumentValidator validator)
            => this.validator = validator;

        public (bool Ok, JObject Collections) Parse(JObject parameters)
        {
            if (parameters.Count != 1)
            {
                return default;
            };

            return (true, parameters);
        }
    }
}
