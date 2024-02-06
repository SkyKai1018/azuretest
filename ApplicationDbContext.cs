using Microsoft.EntityFrameworkCore;
using azuretest.Models;

namespace azuretest.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Stock> Stocks { get; set; }
        public DbSet<TradingData> TradingDatas { get; set; }
        public DbSet<EarningsDistribution> EarningsDistributions { get; set; }
    }
}

