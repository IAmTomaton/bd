using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DB.Application.Core;

namespace DB.Tests.Unit
{
    public class TestServer : IServer
    {
        private readonly Dictionary<string, Func<string, Task<string>>> handlers = new();

        public void AddHandler(string path, Func<string, Task<string>> handler)
            => handlers[path] = handler;

        public Task<string> HandleAsync(string path, string body)
            => handlers[path].Invoke(body);
    }
}