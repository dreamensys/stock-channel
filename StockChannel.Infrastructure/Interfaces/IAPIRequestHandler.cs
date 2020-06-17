using System.Threading.Tasks;

namespace StockChannel.Infrastructure.Interfaces
{
    public interface IAPIRequestHandler
    {
        Task<T> Get<T>(string url, string format);
    }
}