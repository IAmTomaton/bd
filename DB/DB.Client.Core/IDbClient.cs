using System.Threading.Tasks;

namespace DB.Client.Core
{
    public interface IDbClient
    {
        IDbCollection GetCollection(string collectionName);
        Task<string> BackupAsync();
        Task RestoreAsync(string backup);
    }
}