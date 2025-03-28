using PriceDifferenceService.DTOs;
using PriceDifferenceService.Interfaces;
using PriceDifferenceService.Models;

namespace PriceDifferenceService.Services
{
    public class PriceDifferenceService : IPriceDifferenceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly Serilog.ILogger _logger;

        public PriceDifferenceService(IUnitOfWork unitOfWork, Serilog.ILogger logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task CalculateAndSaveDifferenceAsync(List<PriceDto> prices)
        {
            if (prices.Count < 2)
            {
                _logger.Warning("Not enough prices to calculate the difference.");
                return;
            }

            var difference = Math.Abs(prices[0].Price - prices[1].Price);
            var priceDifference = new PriceDifference
            {
                Difference = difference,
                Timestamp = DateTime.UtcNow
            };

            await _unitOfWork.PriceDifferences.AddAsync(priceDifference);
            await _unitOfWork.SaveAsync();
            _logger.Information("Price difference saved: {Difference}", difference);
        }
    }
}
