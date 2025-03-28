using PriceDifferenceService.Models;

namespace PriceDifferenceService.Interfaces
{
    public interface IPriceDifferenceRepository
    {
        Task AddAsync(PriceDifference priceDifference);
        Task<PriceDifference?> GetLastAsync();
    }
}
