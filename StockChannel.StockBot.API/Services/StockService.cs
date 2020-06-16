using System;
using System.Threading.Tasks;
using StockChannel.Domain.Entities;
using StockChannel.Infrastructure.Interfaces;

namespace StockChannel.StockBot.API.Services
{
    public class StockService : IStockService
    {
        private readonly IAPIRequestHandler _requestHandler;
        private const string STOCKS_URL = @"https://stooq.com/q/l/?s=aapl.us&f=sd2t2ohlcv&h&e=csv";
        
        public StockService(IAPIRequestHandler requestHandler)
        {
            _requestHandler = requestHandler ?? throw new ArgumentNullException(nameof(requestHandler));
        }
        
        public async Task<string> GetStockQuote(string stock_code)
        {
            var result = string.Empty;
            //var stock = await _requestHandler.Get<Stock>(STOCKS_URL);
            var now = DateTime.Now;
            var stock = new Stock()
            {
                Symbol = "AAPL.US",
                Date = now,
                Time = new TimeSpan(now.Ticks),
                Open = 344.72,
                High = 344.72,
                Low = 344.72,
                Close = 344.72,
                Volume = 234234
            };
            return BuildMessage(stock);
        }

        private string BuildMessage(Stock model)
        {
            //APPL.US quote is $93.42 per share
            return $"{model.Symbol} quote is ${model.Close} per share.";
        }
    }
}