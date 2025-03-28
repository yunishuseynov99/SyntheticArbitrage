using PriceDifferenceService.DTOs;

namespace PriceDifferenceService.Interfaces
{
    public interface IPriceDifferenceService
    {
        Task CalculateAndSaveDifferenceAsync(List<PriceDto> prices);
    }
}
