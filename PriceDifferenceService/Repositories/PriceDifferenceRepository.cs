using Microsoft.EntityFrameworkCore;
using PriceDifferenceService.Data;
using PriceDifferenceService.Interfaces;
using PriceDifferenceService.Models;

namespace PriceDifferenceService.Repositories
{
    public class PriceDifferenceRepository : IPriceDifferenceRepository
    {
        private readonly PriceDifferenceDbContext _context;

        public PriceDifferenceRepository(PriceDifferenceDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(PriceDifference priceDifference)
        {
            await _context.PriceDifferences.AddAsync(priceDifference);
        }

        public async Task<PriceDifference?> GetLastAsync()
        {
            return await _context.PriceDifferences
                                 .OrderByDescending(x => x.Timestamp)
                                 .FirstOrDefaultAsync();
        }
    }
}
