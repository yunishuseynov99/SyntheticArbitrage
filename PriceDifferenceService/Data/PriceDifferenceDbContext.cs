using Microsoft.EntityFrameworkCore;
using PriceDifferenceService.Models;

namespace PriceDifferenceService.Data
{
    public class PriceDifferenceDbContext : DbContext
    {
        public PriceDifferenceDbContext(DbContextOptions<PriceDifferenceDbContext> options)
            : base(options) { }

        public DbSet<PriceDifference> PriceDifferences { get; set; }
    }
}
