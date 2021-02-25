using DB.Core.State;
using Newtonsoft.Json.Linq;

namespace DB.Core.Commands.Find
{
    public interface IFindCommandExecutor
    {
        bool CanExecute(JToken parameters);
        JObject Execute(IDbState state, string collectionName, JToken parameters);
    }
}