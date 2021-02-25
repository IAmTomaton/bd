using System.Threading.Tasks;

namespace DB.Client.Core
{
    public interface IDbIndexManager
    {
        Task AddAsync(string field);
        Task DropAsync(string field);
    }
}