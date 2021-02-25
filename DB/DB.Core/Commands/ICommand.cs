using DB.Core.State;
using Newtonsoft.Json.Linq;

namespace DB.Core.Commands
{
    public interface ICommand
    {
        string Name { get; }
        JObject Execute(IDbState state, JObject parameters);
    }
}