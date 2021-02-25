using System.Collections.Concurrent;

namespace DB.Core.State
{
    public interface IDbState
    {
        ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentDictionary<string, string>>> Collections { get; }
    }
}