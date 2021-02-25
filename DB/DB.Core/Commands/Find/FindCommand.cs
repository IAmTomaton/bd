using System.Linq;
using DB.Core.Helpers;
using DB.Core.State;
using Newtonsoft.Json.Linq;

namespace DB.Core.Commands.Find
{
    public class FindCommand : ICommand
    {
        private readonly IFindCommandExecutor[] executors;

        public FindCommand(IFindCommandExecutor[] executors)
            => this.executors = executors;

        public string Name => "find";

        public JObject Execute(IDbState state, JObject parameters)
        {
            if (parameters.Count != 1)
            {
                return Result.Error.InvalidRequest;
            }

            var collectionProperty = parameters.Properties().First();
            var collectionName = collectionProperty.Name;

            if (collectionProperty.Count != 1)
            {
                return Result.Error.InvalidRequest;
            }

            foreach (var executor in executors)
            {
                if (executor.CanExecute(collectionProperty.Value))
                {
                    return executor.Execute(state, collectionName, collectionProperty.Value);
                }
            }

            return Result.Error.InvalidRequest;
        }
    }
}