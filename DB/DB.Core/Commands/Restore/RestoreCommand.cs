using System.Collections.Concurrent;
using System.Linq;
using DB.Core.Helpers;
using DB.Core.State;
using Newtonsoft.Json.Linq;

namespace DB.Core.Commands.Restore
{
    public class RestoreCommand : ICommand
    {
        private readonly IRestoreCommandParser parser;

        public RestoreCommand(IRestoreCommandParser parser)
            => this.parser = parser;

        public string Name => "insert";

        public JObject Execute(IDbState state, JObject parameters)
        {
            var (ok, collections) = parser.Parse(parameters);

            if (!ok)
            {
                return Result.Error.InvalidRequest;
            }

            state.Collections.Clear();
            state.Collections.

            return Result.Ok.Empty;
        }
    }
}
