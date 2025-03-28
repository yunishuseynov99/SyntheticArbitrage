namespace PriceDifferenceService.Interfaces
{
    public interface IUnitOfWork
    {
        IPriceDifferenceRepository PriceDifferences { get; }
        Task SaveAsync();
    }
}
