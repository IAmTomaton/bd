using System;
using System.Threading.Tasks;

namespace DB.Application.Core
{
    public interface IServer
    {
        void AddHandler(string path, Func<string, Task<string>> handler);
    }
}