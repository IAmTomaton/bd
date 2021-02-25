using System.Threading.Tasks;
using DB.Client.Core.Helpers;

namespace DB.Client.Core
{
    public class DbClient : IDbClient
    {
        private readonly IDbRequestSender sender;

        public DbClient(IDbRequestSender sender)
            => this.sender = sender;

        public IDbCollection GetCollection(string collectionName)
            => new DbCollection(collectionName, sender);

        public async Task<string> BackupAsync()
        {
            var rawResult = await sender.SendAsync(CommandBuilder.Build("backup")).ConfigureAwait(false);
            return ResultParser.Parse(rawResult).ToString();
        }

        public async Task RestoreAsync(string backup)
        {
            var rawResult = await sender.SendAsync(CommandBuilder.Build("restore", backup)).ConfigureAwait(false);
            ResultParser.Parse(rawResult);
        }
    }
}