using FuturesPriceService.DTOs;

namespace FuturesPriceService.Interfaces
{
    public interface IFuturesPriceService
    {
        Task<List<PriceDto>> GetPricesAsync();
        List<PriceDto>? GetLastSuccessfulPrices();
    }
}
