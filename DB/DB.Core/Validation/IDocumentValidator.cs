using Newtonsoft.Json.Linq;

namespace DB.Core.Validation
{
    public interface IDocumentValidator
    {
        bool IsValid(JObject document);
    }
}