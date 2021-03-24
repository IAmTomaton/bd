using System.Collections.Generic;
using DB.Core;
using DB.Core.Commands;
using DB.Core.Commands.Delete;
using DB.Core.Commands.Find;
using DB.Core.Commands.Insert;
using DB.Core.Commands.Replace;
using DB.Core.Commands.Backup;
using DB.Core.Parsing;
using DB.Core.State;
using DB.Core.Validation;
using DB.Core.Commands.Restore;
using DB.Core.Commands.Update;
using DB.Core.Commands.Indexes;

namespace DB.Application.Core
{
    public static class DbFactory
    {
        public static IDb Create()
        {
            var validator = new DocumentValidator();
            var commands = new List<ICommand>
            {
                new InsertCommand(new InsertCommandParser(validator)),
                new ReplaceCommand(new ReplaceCommandParser(validator)),
                new FindCommand(new IFindCommandExecutor[]
                {
                    new FindByIdCommandExecutor(),
                    new FindByFieldCommandExecutor()
                }),
                new DeleteCommand(),
                new BackupCommand(),
                new RestoreCommand(new RestoreCommandParser(validator)),
                new UpdateCommand(new UpdateCommandParser(), new IUpdateCommandExecutor[]
                {
                    new SetCommandExecutor(),
                    new UnsetCommandExecutor()
                }),
                new AddIndexCommand(),
                new DropIndexCommand()
            };
            var parser = new DbCommandParser();
            var state = new DbState();

            return new Db(state, parser, commands);
        }
    }
}