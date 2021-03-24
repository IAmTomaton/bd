using DB.Core.Helpers;
using DB.Core.State;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace DB.Core.Commands.Update
{
    public class UpdateCommand : ICommand
    {
        private readonly IUpdateCommandParser parser;
        private readonly IUpdateCommandExecutor[] executors;

        public UpdateCommand(IUpdateCommandParser parser, IUpdateCommandExecutor[] executors)
        {
            this.parser = parser;
            this.executors = executors;
        }

        public string Name => "update";

        public JObject Execute(IDbState state, JObject parameters)
        {
            var (ok, collectionName, id, commands) = parser.Parse(parameters);

            if (!ok)
                return Result.Error.InvalidRequest;

            foreach (var command in commands)
            {
                if (!executors.Any(executor => executor.CanExecute(command)))
                    return Result.Error.InvalidRequest;
            }

            if (!state.Collections.TryGetValue(collectionName, out var collection))
                return Result.Error.NotFound;

            if (!collection.TryGetValue(id, out var document))
                return Result.Error.NotFound;

            foreach (var command in commands)
            {
                foreach (var executor in executors)
                {
                    if (executor.CanExecute(command))
                        executor.Execute(state, collectionName, id, document, command);
                }
            }

            return Result.Ok.Empty;
        }
    }
}
