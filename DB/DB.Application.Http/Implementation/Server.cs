using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using DB.Application.Core;

namespace DB.Application.Http.Implementation
{
    public class Server : IServer
    {
        private readonly ConcurrentDictionary<string, Func<string, Task<string>>> handlers = new()
        {
            ["/ping"] = _ => Task.FromResult("pong")
        };
        
        public void AddHandler(string path, Func<string, Task<string>> handler)
            => handlers[path] = handler;

        public Task<string> HandleAsync(string path, string body)
            => handlers[path.ToLower()].Invoke(body);
    }
}