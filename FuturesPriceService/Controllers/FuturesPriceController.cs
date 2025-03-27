using FuturesPriceService.DTOs;
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
        /// <summary>
        /// Retrieves the latest prices for all configured futures contracts.
        /// </summary>
        /// <returns>A list of FuturesPriceDto objects containing contract and price information.</returns>
        /// <response code="200">Returns a list of futures prices</response>
        /// <response code="404">If no prices are found</response>
        [HttpGet("get-prices")]
        [ProducesResponseType(typeof(List<PriceDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetPrices()
        {
            var prices = await _futuresPriceService.GetPricesAsync();

            if (prices == null || !prices.Any())
                return NotFound("No prices found.");

            return Ok(prices);
        }
    }
}
