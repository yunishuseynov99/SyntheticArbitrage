using PriceDifferenceService.Data;
using PriceDifferenceService.Interfaces;

namespace PriceDifferenceService.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PriceDifferenceDbContext _context;

        public UnitOfWork(PriceDifferenceDbContext context)
        {
            _context = context;
            PriceDifferences = new PriceDifferenceRepository(_context);
        }

        public IPriceDifferenceRepository PriceDifferences { get; }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
