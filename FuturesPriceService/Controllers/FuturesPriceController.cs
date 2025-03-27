using FuturesPriceService.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FuturesPriceService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FuturesPriceController : ControllerBase
    {
        private readonly IFuturesPriceService _futuresPriceService;

        public FuturesPriceController(IFuturesPriceService futuresPriceService)
        {
            _futuresPriceService = futuresPriceService;
        }

        [HttpGet("get-prices")]
        public async Task<IActionResult> GetPrices()
        {
            var prices = await _futuresPriceService.GetPricesAsync();

            if (prices == null || !prices.Any())
                return NotFound("No prices found.");

            return Ok(prices);
        }
    }
}
