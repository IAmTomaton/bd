using System.Collections.Generic;
using System.Threading.Tasks;
using DB.Client.Core.UpdateDefinitions;

namespace DB.Client.Core
{
    public interface IDbCollection
    {
        IDbIndexManager Indexes { get; }

        Task InsertAsync(string id, Dictionary<string, string> document);
        Task<Dictionary<string, string>> FindAsync(string id);
        Task<List<(string Id, Dictionary<string, string> Document)>> FindAsync(string field, string value);
        Task ReplaceAsync(string id, Dictionary<string, string> document, bool upsert = false);
        Task UpdateAsync(string id, IUpdateDefinition updateDefinition);
        Task DeleteAsync(string id);
    }
}