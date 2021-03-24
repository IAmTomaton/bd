using System.Collections.Concurrent;
using System.Collections.Generic;

namespace DB.Core.State
{
    public interface IDbState
    {
        ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentDictionary<string, string>>> Collections { get; }
        ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentDictionary<string, List<string>>>> Indexes { get; }
    }
}