using System.Threading.Tasks;
using DB.Client.Core.Helpers;

namespace DB.Client.Core
{
    internal class DbIndexManager : IDbIndexManager
    {
        private readonly string collectionName;
        private readonly IDbRequestSender sender;

        public DbIndexManager(string collectionName, IDbRequestSender sender)
        {
            this.collectionName = collectionName;
            this.sender = sender;
        }

        public async Task AddAsync(string field)
        {
            var command = CommandBuilder.BuildAddIndex(collectionName, field);
            var rawResult = await sender.SendAsync(command).ConfigureAwait(false);
            ResultParser.Parse(rawResult);
        }

        public async Task DropAsync(string field)
        {
            var command = CommandBuilder.BuildDropIndex(collectionName, field);
            var rawResult = await sender.SendAsync(command).ConfigureAwait(false);
            ResultParser.Parse(rawResult);
        }
    }
}