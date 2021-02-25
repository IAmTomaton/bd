using System;
using System.Collections.Generic;
using System.Linq;
using DB.Core.Commands;
using DB.Core.Helpers;
using DB.Core.Parsing;
using DB.Core.State;
using Newtonsoft.Json.Linq;

namespace DB.Core
{
    public class Db : IDb
    {
        private readonly object lockObject = new();

        private readonly IDbState state;
        private readonly IDbCommandParser dbCommandParser;
        private readonly Dictionary<string, ICommand> commands;

        public Db(IDbState state, IDbCommandParser dbCommandParser, IEnumerable<ICommand> commands)
        {
            this.state = state;
            this.dbCommandParser = dbCommandParser;
            this.commands = commands.ToDictionary(x => x.Name);
        }

        public string Execute(string input)
        {
            try
            {
                return ExecuteInternal(input).ToString();
            }
            catch (Exception e)
            {
                return Result.Error.WithMessage(e.Message).ToString();
            }
        }

        private JObject ExecuteInternal(string input)
        {
            lock (lockObject)
            {
                var (ok, commandName, parameters) = dbCommandParser.Parse(input);
                if (!ok)
                {
                    return Result.Error.InvalidRequest;
                }

                if (!commands.TryGetValue(commandName, out var command))
                {
                    return Result.Error.CommandNotFound;
                }

                return command.Execute(state, parameters);
            }
        }
    }
}