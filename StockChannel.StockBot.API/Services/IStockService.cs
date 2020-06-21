using System.Threading.Tasks;

namespace StockChannel.StockBot.API.Services
{
    public interface IStockService
    {
        Task<string> GetStockQuote(string stock_code);
    }
}