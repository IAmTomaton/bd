using DB.Core.Helpers;
using DB.Core.State;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace DB.Core.Commands.Restore
{
    public class RestoreCommand : ICommand
    {
        private readonly IRestoreCommandParser parser;

        public RestoreCommand(IRestoreCommandParser parser)
            => this.parser = parser;

        public string Name => "restore";

        public JObject Execute(IDbState state, JObject parameters)
        {
            var (ok, tables) = parser.Parse(parameters);

            if (!ok)
            {
                return Result.Error.InvalidRequest;
            }

            state.Collections.Clear();

            foreach (var keyTablePair in tables)
            {
                state.Collections[keyTablePair.Key] = keyTablePair.Value;
            }
            return Result.Ok.Empty;
        }
    }
}
