using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace DB.Core.State
{
    public class DbState : IDbState
    {
        public ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentDictionary<string, string>>> Collections { get; } = new();
        public ConcurrentDictionary<string, ConcurrentDictionary<string, Tuple<List<string>, List<string>>>> Indexes { get; } = new();
    }
}