using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockChannel.StockBot.API.Services;

namespace StockChannel.StockBot.API.Controllers
{
    [Route("api/bots/stock")]
    [ApiController]
    public class StockBotController : ControllerBase
    {
        private readonly IStockService _stockService;
        
        public StockBotController(IStockService stockService)
        {
            _stockService = stockService;
        }
        
        /// <summary>
        /// Bot Controller
        /// </summary>
        /// <param name="stockCode"></param>
        /// <returns></returns>
        [HttpGet("{stockCode}")]
        public async Task<IActionResult> GetStockQuote(string stockCode)
        {
            try
            {
                var result = await _stockService.GetStockQuote(stockCode);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
    }
}