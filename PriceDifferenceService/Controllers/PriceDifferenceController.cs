using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PriceDifferenceService.DTOs;
using PriceDifferenceService.Interfaces;

namespace PriceDifferenceService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PriceDifferenceController : ControllerBase
    {
        private readonly IPriceDifferenceService _priceDifferenceService;

        public PriceDifferenceController(IPriceDifferenceService priceDifferenceService)
        {
            _priceDifferenceService = priceDifferenceService;
        }

        [HttpPost("calculate")]
        public async Task<IActionResult> CalculateDifference([FromBody] List<PriceDto> prices)
        {
            if (prices == null || prices.Count < 2)
            {
                return BadRequest("At least two prices are required.");
            }

            await _priceDifferenceService.CalculateAndSaveDifferenceAsync(prices);
            return Ok("Difference calculated and saved.");
        }
    }

}
