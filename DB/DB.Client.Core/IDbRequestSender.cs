using System.Threading.Tasks;

namespace DB.Client.Core
{
    public interface IDbRequestSender
    {
        Task<string> SendAsync(string command);
    }
}