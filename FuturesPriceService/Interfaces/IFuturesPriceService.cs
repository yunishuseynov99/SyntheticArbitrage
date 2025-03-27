using FuturesPriceService.DTOs;

namespace FuturesPriceService.Interfaces
{
    public interface IFuturesPriceService
    {
        Task<List<FuturesPriceDto>> GetPricesAsync();
    }
}
