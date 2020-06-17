using System;
using System.Threading.Tasks;
using StockChannel.Domain.Entities;
using StockChannel.Infrastructure.Interfaces;

namespace StockChannel.StockBot.API.Services
{
    public class StockService : IStockService
    {
        private readonly IAPIRequestHandler _requestHandler;
        
        public StockService(IAPIRequestHandler requestHandler)
        {
            _requestHandler = requestHandler ?? throw new ArgumentNullException(nameof(requestHandler));
        }
        
        public async Task<string> GetStockQuote(string stockCode)
        {
            var url = @getStockURL(stockCode);
            Stock stock;
            try
            {
                stock = await _requestHandler.Get<Stock>(url, "CSV");
            }
            catch
            {
                return $"No data found for {stockCode}";
            }
            
            return BuildMessage(stock);
        }

        private string BuildMessage(Stock model) => $"{model.Symbol} quote is ${model.Close} per share.";
        private string getStockURL(string stockCode) => @$"https://stooq.com/q/l/?s={stockCode}&f=sd2t2ohlcv&h&e=csv";

    }
}